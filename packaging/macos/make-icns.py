#!/usr/bin/env python3
"""Gera traderock.icns (ícone do RockAssistent.app, D35) com o mesmo desenho do packaging/windows/make-icon.py,
sem ferramentas externas: cada tamanho é um PNG dentro do .icns. Como nos ícones do macOS, o desenho ocupa 80% do
quadro, com margem transparente.

Se o icon.svg mudar, atualize o make-icon.py do Windows e rode: python3 packaging/macos/make-icns.py
"""

import importlib.util
import os
import struct
import zlib

HERE = os.path.dirname(os.path.abspath(__file__))
spec = importlib.util.spec_from_file_location("make_icon", os.path.join(HERE, "..", "windows", "make-icon.py"))
icon = importlib.util.module_from_spec(spec)
spec.loader.exec_module(icon)

# Tipos do .icns com PNG: (tipo, lado em pixels).
TYPES = [(b"ic11", 32), (b"ic12", 64), (b"ic07", 128), (b"ic08", 256), (b"ic09", 512)]
SCALE = 0.8


def render(size):
    samples = 4 if size <= 128 else 2
    margin = size * (1 - SCALE) / 2
    rows = []
    for py in range(size):
        row = bytearray([0])
        for px in range(size):
            acc = [0, 0, 0]
            hits = 0
            for sy in range(samples):
                for sx in range(samples):
                    x = (px + (sx + 0.5) / samples - margin) * 64 / (size * SCALE)
                    y = (py + (sy + 0.5) / samples - margin) * 64 / (size * SCALE)
                    c = icon.color_at(x, y)
                    if c:
                        hits += 1
                        for i in range(3):
                            acc[i] += c[i]
            if hits:
                row += bytes(round(a / hits) for a in acc) + bytes([round(255 * hits / samples ** 2)])
            else:
                row += b"\0\0\0\0"
        rows.append(bytes(row))

    def chunk(kind, data):
        return struct.pack(">I", len(data)) + kind + data + struct.pack(">I", zlib.crc32(kind + data))

    header = struct.pack(">IIBBBBB", size, size, 8, 6, 0, 0, 0)
    return b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", header) + chunk(b"IDAT", zlib.compress(b"".join(rows), 9)) + chunk(b"IEND", b"")


def main():
    body = b"".join(kind + struct.pack(">I", 8 + len(png)) + png for kind, png in ((k, render(s)) for k, s in TYPES))
    path = os.path.join(HERE, "traderock.icns")
    with open(path, "wb") as f:
        f.write(b"icns" + struct.pack(">I", 8 + len(body)) + body)
    print(path)


if __name__ == "__main__":
    main()
