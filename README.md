# GHOT to GH3 Converter
Application to batch convert pre-decrypted files from the Guitar Hero On Tour series to Guitar Hero III-format arrays for Queen Bee

Works with:
- Guitar Hero: On Tour
- Guitar Hero On Tour: Decades
- Guitar Hero On Tour: Modern Hits
- Band Hero DS (Guitar and Bass charts only)

Files will need to be decrypted with DSDecmpGH before they can run through the GHOTtoGH3 program.
Once decrypted, drag the song files onto the GHOTtoGH3 executable and it will create a new directory where the .array.text files will be created, which can then be imported into programs such as Queen Bee or EOF.

Only the frets, gems, gems_bass, sections_English and timesig files are required to build the song data. All other files will be ignored.
