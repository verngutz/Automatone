:: Replace gcc below with whatever C compiler you use, including the path
:: You will also need Midi2Mtx.h
gcc.exe crack.c Midi2Mtx.c ReadMidiFile.c WriteMtxfile.c -o Midi2Mtx.exe
:: Replace strip below with your strip program, including the path
:: This step in not strictly necessary, but can make the program size much smaller
strip.exe Midi2Mtx.exe
