from PIL import Image
import os

# Avaa kuva
kuva = Image.open("Map_of_Finland.png")
kuva_leveys, kuva_korkeus = kuva.size

# Määritä palasten koko
sarakkeet = 3
rivit = 5
palan_leveys = kuva_leveys // sarakkeet
palan_korkeus = kuva_korkeus // rivit

# Luo kansio tallennusta varten
os.makedirs("palat", exist_ok=True)

# Pilko kuva
for rivi in range(rivit):
    for sarake in range(sarakkeet):
        vasen = sarake * palan_leveys
        yla = rivi * palan_korkeus
        oikea = vasen + palan_leveys
        ala = yla + palan_korkeus

        pala = kuva.crop((vasen, yla, oikea, ala))
        pala.save(f"palat/pala_{rivi}_{sarake}.png")

print("Pilkkoutuminen valmis!")