import VaultPage from '../vault/VaultPage'

/** Aba "Cofre do app" do Boxes: o cofre de antes, sem o título próprio (a página já tem o "Boxes"). */
export function VaultTab({ savesAvailable }: { savesAvailable: boolean }) {
  return <VaultPage embedded savesAvailable={savesAvailable} />
}
