# RockAssistent: comandos de release. Rode na raiz do repositório (Linux ou WSL).
#
#   make tag    publica uma versão nova: mostra o que entrou na main, pergunta o tipo e as Novidades e envia a tag
#               (o CI testa, gera os pacotes e publica o Release)

.PHONY: tag
tag:
	@bash scripts/tag.sh
