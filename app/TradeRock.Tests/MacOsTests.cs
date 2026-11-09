using TradeRock.Core;
using TradeRock.Core.Esp32;
using TradeRock.Core.Setup;
using TradeRock.Core.Updates;

namespace TradeRock.Tests;

/// <summary>macOS (D35): o que dá para conferir fora de um Mac.</summary>
public class MacOsTests
{
    // Trecho no formato do "ioreg -r -c IOUSBHostDevice -l": um ESP32-S3 pela USB nativa e um CP2102.
    private const string Ioreg = """
        +-o USB JTAG/serial debug unit@01100000  <class IOUSBHostDevice, id 0x100000a1b, registered, matched, active, busy 0 (5 ms), retain 30>
          | {
          |   "USB Product Name" = "USB JTAG/serial debug unit"
          |   "idProduct" = 4097
          |   "idVendor" = 12346
          |   "USB Vendor Name" = "Espressif"
          | }
          |
          +-o IOUSBHostInterface@0  <class IOUSBHostInterface, id 0x100000a20>
            +-o AppleUSBACMData  <class AppleUSBACMData, id 0x100000a2b>
              +-o IOModemSerialStreamSync  <class IOModemSerialStreamSync, id 0x100000a2d>
                +-o IOSerialBSDClient  <class IOSerialBSDClient, id 0x100000a2e>
                    {
                      "IOCalloutDevice" = "/dev/cu.usbmodem1101"
                      "IODialinDevice" = "/dev/tty.usbmodem1101"
                    }
        +-o CP2102 USB to UART Bridge Controller@01200000  <class IOUSBHostDevice, id 0x100000b00>
          | {
          |   "idVendor" = 4292
          |   "idProduct" = 60000
          |   "USB Product Name" = "CP2102 USB to UART Bridge Controller"
          | }
          +-o IOSerialBSDClient  <class IOSerialBSDClient, id 0x100000b10>
              {
                "IOCalloutDevice" = "/dev/cu.usbserial-0001"
              }
        +-o Teclado@01300000  <class IOUSBHostDevice, id 0x100000c00>
          | {
          |   "idVendor" = 1452
          |   "idProduct" = 834
          | }
        """;

    [Fact]
    public void Portas_do_ioreg_com_vid_e_pid()
    {
        var ports = MacProbe.Ports(
            ["/dev/cu.usbmodem1101", "/dev/cu.usbserial-0001", "/dev/cu.Bluetooth-Incoming-Port", "/dev/cu.debug-console", "/dev/cu.wchusbserial120"],
            Ioreg);
        Assert.Equal(["/dev/cu.usbmodem1101", "/dev/cu.usbserial-0001", "/dev/cu.wchusbserial120"], ports.Select(p => p.Path));
        Assert.Equal(("303a", "1001"), (ports[0].VendorId, ports[0].ProductId));
        Assert.Equal(("10c4", "ea60"), (ports[1].VendorId, ports[1].ProductId));
        Assert.Equal("CP2102 USB to UART Bridge Controller", ports[1].ProductName);
        Assert.Null(ports[2].VendorId); // o ioreg não falou dela: só o nome
    }

    [Fact]
    public void Sem_ioreg_ficam_so_os_nomes()
    {
        var port = Assert.Single(MacProbe.Ports(["/dev/cu.usbmodem1101"], null));
        Assert.Equal(new SerialPortInfo("/dev/cu.usbmodem1101", null, null, null), port);
    }

    [Fact]
    public void No_mac_o_radio_e_so_o_esp32()
    {
        var system = SetupScenarios.All["macos-esp32"];
        var advice = RadioAdvisor.Advise(system, HardwareCatalog.Embedded);
        Assert.Equal("macos", advice.Situation);
        Assert.False(advice.Options.Single(o => o.Engine == "wifi").Available);
        Assert.Equal("macos", advice.Options.Single(o => o.Engine == "wifi").ReasonCode);
        Assert.Equal("esp32", Assert.Single(advice.Options, o => o.Recommended).Engine);

        var checks = SetupChecks.Evaluate(system, advice,
            new SetupState { Radio = "esp32", Esp32Board = "esp32-s3", SerialPort = "/dev/cu.usbmodem1101" }, null, HardwareCatalog.Embedded);
        Assert.Equal("serial_ok", checks.Single(c => c.Id == "serial_port").Code);
        Assert.DoesNotContain(checks, c => c.Id is "serial_group" or "brltty" or "udev_rule" or "usb_driver");

        checks = SetupChecks.Evaluate(system, advice, new SetupState { Radio = "wifi" }, null, HardwareCatalog.Embedded);
        Assert.Equal("not_native_macos", checks.Single(c => c.Id == "native_linux").Code);
    }

    [Fact]
    public void Pasta_de_dados_no_application_support() =>
        Assert.Equal("/Users/ash/Library/Application Support/TradeRock", AppPaths.MacDataDir("/Users/ash"));

    [Fact]
    public void Runtime_do_esp32_em_contents_resources() =>
        Assert.Equal("/Applications/RockAssistent.app/Contents/Resources/esp32-runtime",
            PythonEsp32Flasher.MacBundleRuntime("/Applications/RockAssistent.app/Contents/MacOS/"));

    [Theory]
    [InlineData("/Applications/RockAssistent.app/Contents/MacOS/", "/Applications/RockAssistent.app")]
    [InlineData("/Users/ash/Applications/RockAssistent.app/Contents/MacOS", "/Users/ash/Applications/RockAssistent.app")]
    [InlineData("/Volumes/RockAssistent/RockAssistent.app/Contents/MacOS/", null)] // rodando de dentro do .dmg
    [InlineData("/private/var/folders/x/AppTranslocation/ABC/d/RockAssistent.app/Contents/MacOS/", null)]
    [InlineData("/home/ash/traderock/", null)]
    public void Pacote_app_do_executavel(string baseDirectory, string? bundle) =>
        Assert.Equal(bundle, UpdateInstallers.MacBundleOf(baseDirectory));

    [Fact]
    public void Arquivo_do_release_do_mac() =>
        Assert.Equal("RockAssistent-1.2.3-macOS-arm64.dmg", AppVersions.AssetName(InstallKind.MacApp, "1.2.3"));

    [Fact]
    public async Task Atualizacao_troca_o_app_pelo_do_dmg()
    {
        var root = Directory.CreateTempSubdirectory("traderock-mac-").FullName;
        try
        {
            var bundle = Path.Combine(root, "RockAssistent.app");
            Directory.CreateDirectory(Path.Combine(bundle, "Contents", "MacOS"));
            File.WriteAllText(Path.Combine(bundle, "Contents", "MacOS", "traderock"), "velho");
            var calls = new List<string>();
            string? relaunched = null;

            Task<(int, string)> Run(string program, IReadOnlyList<string> args)
            {
                calls.Add(program.EndsWith("ditto") ? "ditto" : $"{Path.GetFileName(program)} {args[0]}");
                if (program.EndsWith("hdiutil") && args[0] == "attach")
                {
                    // O "disco" montado: o app novo dentro dele.
                    var mount = args[^2];
                    Directory.CreateDirectory(Path.Combine(mount, "RockAssistent.app", "Contents", "MacOS"));
                    File.WriteAllText(Path.Combine(mount, "RockAssistent.app", "Contents", "MacOS", "traderock"), "novo");
                }
                else if (program.EndsWith("hdiutil") && args[0] == "detach")
                {
                    Directory.Delete(Path.Combine(args[^1], "RockAssistent.app"), recursive: true);
                }
                else if (program.EndsWith("ditto"))
                {
                    var target = Path.Combine(args[1], "Contents", "MacOS");
                    Directory.CreateDirectory(target);
                    File.Copy(Path.Combine(args[0], "Contents", "MacOS", "traderock"), Path.Combine(target, "traderock"));
                }
                return Task.FromResult((0, ""));
            }

            var installer = new MacAppInstaller(bundle, Run, b => relaunched = b);
            await installer.InstallAsync(Path.Combine(root, "RockAssistent-1.2.3-macOS-arm64.dmg"), "x", CancellationToken.None);

            Assert.Equal("novo", File.ReadAllText(Path.Combine(bundle, "Contents", "MacOS", "traderock")));
            Assert.Equal(["RockAssistent.app"], Directory.EnumerateFileSystemEntries(root).Select(Path.GetFileName));
            Assert.Equal(["hdiutil attach", "ditto", "hdiutil detach", "xattr -dr"], calls);
            Assert.Equal(bundle, relaunched);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Sem_permissao_o_app_antigo_fica()
    {
        var root = Directory.CreateTempSubdirectory("traderock-mac-").FullName;
        try
        {
            var bundle = Path.Combine(root, "RockAssistent.app");
            Directory.CreateDirectory(bundle);
            var relaunched = false;
            Task<(int, string)> Run(string program, IReadOnlyList<string> args)
            {
                if (program.EndsWith("hdiutil") && args[0] == "attach")
                    Directory.CreateDirectory(Path.Combine(args[^2], "RockAssistent.app"));
                if (program.EndsWith("hdiutil") && args[0] == "detach")
                    Directory.Delete(Path.Combine(args[^1], "RockAssistent.app"));
                return Task.FromResult(program.EndsWith("ditto") ? (1, "Permission denied") : (0, ""));
            }

            var installer = new MacAppInstaller(bundle, Run, _ => relaunched = true);
            var error = await Assert.ThrowsAsync<UpdateException>(() =>
                installer.InstallAsync(Path.Combine(root, "x.dmg"), "x", CancellationToken.None));
            Assert.Equal("update_no_permission", error.Code);
            Assert.True(Directory.Exists(bundle));
            Assert.False(relaunched);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
