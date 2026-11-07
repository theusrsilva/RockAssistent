#!/usr/bin/env python3
"""Gera traderock.ico a partir do desenho de frontend/public/icon.svg (uma Poké Bola com um "R"; o mesmo
desenho de packaging/linux/io.github.traderock.TradeRock.svg), sem
ferramentas externas: cada tamanho é desenhado com supersampling e gravado como PNG dentro do .ico.

Se o icon.svg mudar, atualize as formas abaixo e rode: python3 packaging/windows/make-icon.py
"""

import os
import struct
import zlib

RED = (0xEF, 0x44, 0x44)
WHITE = (0xF8, 0xFA, 0xFC)
DARK = (0x1E, 0x29, 0x3B)
SIZES = [16, 24, 32, 48, 64, 256]
SAMPLES = 4  # 4x4 amostras por pixel (bordas suaves)

# O "R" do botão: o traço do <path> (stroke-width 3.4, pontas e cantos redondos), como segmentos e um arco.
R_HALF = 1.7
R_SEGMENTS = [((28, 39.5), (28, 24)), ((28, 24), (32, 24)), ((32, 33), (28, 33)), ((31, 33), (36, 39.5))]
R_ARC = ((32, 28.5), 4.5)  # a 4.5 4.5 de (32,24) a (32,33): metade da direita


def dist_segment(x, y, a, b):
    (ax, ay), (bx, by) = a, b
    dx, dy = bx - ax, by - ay
    t = max(0.0, min(1.0, ((x - ax) * dx + (y - ay) * dy) / (dx * dx + dy * dy)))
    return ((x - ax - t * dx) ** 2 + (y - ay - t * dy) ** 2) ** 0.5


def in_r(x, y):
    if any(dist_segment(x, y, a, b) <= R_HALF for a, b in R_SEGMENTS):
        return True
    (cx, cy), radius = R_ARC
    d = ((x - cx) ** 2 + (y - cy) ** 2) ** 0.5
    return x >= cx and abs(d - radius) <= R_HALF


def color_at(x, y):
    """Cor no ponto (x, y) do viewBox 0 0 64 64, ou None (transparente). Mesma ordem de pintura do SVG."""
    r = ((x - 32) ** 2 + (y - 32) ** 2) ** 0.5
    color = None
    if r <= 29.5:
        color = RED                      # <circle r=29.5 fill=#ef4444>
        if y >= 32:
            color = WHITE                # <path M2.5 32h59a29.5 29.5 0 0 1-59 0z> (metade de baixo)
    if 2 <= x <= 62 and 29 <= y <= 35 and r <= 31:
        color = DARK                     # <rect x=2 y=29 width=60 height=6>
    if 28 <= r <= 31:
        color = DARK                     # contorno: <circle r=29.5 stroke-width=3>
    if r <= 11:
        color = WHITE                    # botão: <circle r=13 fill=#f8fafc ...>
    if 11 <= r <= 15:
        color = DARK                     # ... stroke=#1e293b stroke-width=4
    if in_r(x, y):
        color = DARK                     # o "R" do RockAssistent
    return color


def render(size):
    rows = []
    for py in range(size):
        row = bytearray([0])  # filtro PNG "nenhum"
        for px in range(size):
            acc = [0, 0, 0]
            hits = 0
            for sy in range(SAMPLES):
                for sx in range(SAMPLES):
                    x = (px + (sx + 0.5) / SAMPLES) * 64 / size
                    y = (py + (sy + 0.5) / SAMPLES) * 64 / size
                    c = color_at(x, y)
                    if c:
                        hits += 1
                        for i in range(3):
                            acc[i] += c[i]
            n = SAMPLES * SAMPLES
            if hits:
                row += bytes(round(a / hits) for a in acc) + bytes([round(255 * hits / n)])
            else:
                row += b"\0\0\0\0"
        rows.append(bytes(row))
    raw = b"".join(rows)

    def chunk(kind, data):
        return struct.pack(">I", len(data)) + kind + data + struct.pack(">I", zlib.crc32(kind + data))

    header = struct.pack(">IIBBBBB", size, size, 8, 6, 0, 0, 0)  # RGBA 8 bits
    return b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", header) + chunk(b"IDAT", zlib.compress(raw, 9)) + chunk(b"IEND", b"")


def main():
    images = [render(s) for s in SIZES]
    out = bytearray(struct.pack("<HHH", 0, 1, len(images)))
    offset = 6 + 16 * len(images)
    for size, png in zip(SIZES, images):
        dim = 0 if size == 256 else size
        out += struct.pack("<BBBBHHII", dim, dim, 0, 0, 1, 32, len(png), offset)
        offset += len(png)
    for png in images:
        out += png
    path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "traderock.ico")
    with open(path, "wb") as f:
        f.write(out)
    print(path)


if __name__ == "__main__":
    main()
