:: Replace gcc below with whatever C compiler you use, including the path
:: You'll also need Mtx2Midi.h
gcc.exe crack.c Mtx2Midi.c t2mflex.c translate.c WriteMidifile.c -o Mtx2Midi.exe
:: Replace strip below with your strip program, including the path
:: This step in not strictly necessary, but can make the program size much smaller
strip.exe Mtx2Midi.exe
