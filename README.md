# Aufgabe 2 - Language Model

| Gruppenmitglieder |
|-|
| Robin Augenstein |
| Kai Petelski |
| Jannick Gutekunst |

## TOC

- [Aufgabe 2 - Language Model](#aufgabe-2---language-model)
  - [TOC](#toc)
  - [Bauen der Programme](#bauen-der-programme)
  - [Starten der Programme](#starten-der-programme)
  - [Struktur des Repositories](#struktur-des-repositories)

## Bauen der Programme

Die Anwendung kann mithilfe von Docker gebaut werden. Hierfür in das Hauptverzeichnis des Repositories wechseln und `docker build . -t language_model` ausführen. Hierdurch werden die Programme gebaut und direkt ausführbar in einem Docker-Image bereitgestellt.

Im Image wird kein `ENTRYPOINT` und kein `CMD` gesetzt, damit der Anwender flexibel auswählen kann, welches Programm er mit welchen Dateien bzw. Parametern verwenden möchte.

## Starten der Programme

Auf Basis des zuvor erstellten Docker-Images können die Anwendungungen innerhalb eines Docker-Containers direkt gestartet werden. Nachdem dieses wie beschrieben erstellt wurde, können die Programme mithilfe eines Befehls nach dem Muster `docker run language_model dotnet <Programmname>.dll <Parameter>` gestartet werden. Hierbei müssen die Platzhalter `<Programmname>` und `<Parameter>` durch die gewünschten Werte ersetzt werden.

- Folgende Programme können gestartet werden:

  | Programmname | Beispiel für den Befehl |
  |-|-|
  | `Learn` | `docker run language_model dotnet Learn.dll` |
  | `Perplexity` | `docker run language_model dotnet Perplexity.dll` |

- Die Programme akzeptieren folgende Aufrufparameter:
  - `Learn`

    | Parameter | Bedeutung |
    |-|-|
    | `-i`, `--inputFilePath` | Required. Path to file with input text to learn. |
    | `-o`, `--outputFilePath` | Required. Filepath that the learned language model will be saved to using ARPA representation. If the file already exists, it will be overwritten! |
    | `-s`, `--smoothing` | Required. Type of smoothing to apply. Supported values: "Regular" and "KneserNey" |
    | `-v`, `--verbose` | Enable verbose logging. |
    | `--help` | Display help screen. |

  - `Perplexity`

    | Parameter | Bedeutung |
    |-|-|
    | `-m`, `--model` | Required. Path to a file containing the language model to use for perplexity calculation. Must be in ARPA format. |
    | `-i`, `--text` | Required. The text to calculate the perplexity of. |
    | `-v`, `--verbose` | Enable verbose logging. |
    | `--help` | Display help screen. |

- Beispiel
  - Programm `Learn`
  - Eingabedatei `/data/1_sample_text.txt`
  - Ausgabedatei `/data/out_regular.txt`
  - Smoothing: Regular
  - Datenaustausch zwischen Host und Container über Volume, Pfade zum Volume hier lokal `./data`, das auf den Pfad `/data` des Containers abgebildet wird
  - Als Befehl ergibt sich hieraus:  
    `docker run -v $(pwd)/data:/data language_model dotnet learn.dll -i /data/1_sample_text.txt -o /data/out_regular.arpa -s Regular`

## Struktur des Repositories

| Verzeichnis | Inhalt |
|-|-|
| `.vscode/` | Enthält erforderliche Metadaten für die Verwendung von VS Code als IDE. |
| `LanguageModel/` | Enthält eine selbsterstellte Bibliothek, die Funktionen, die von diesen beiden Programmen genutzt werden, bereitstellt. |
| `LanguageModel/Smoothing/` | Enthält die Implementierung von verschiedenen Smoothing-Verfahren, die bei dem Lernen eines Language Models eingesetzt werden können. Aktuell sind Regular-Smoothing und Kneser-Ney-Smoothing implementiert. |
| `Learn/` | Enthält das Programm, mit dem ein Language Model aus einem Eingabetext gelernt werden kann. |
| `Perplexity/` | Enthält das Programm, mit dem die Perplexität eines Eingabesatzes unter Verwendung eines im ARPA-Format bereitgestellten Language Models bestimmt werden kann. |
| `.gitignore` | gitignore-Datei - definiert, welche Dateien nicht von Git versioniert werden sollen. |
| `Dockerfile` | Ermöglicht die automatisierte Erstellung eines Docker-Images, in dem die Programme direkt ausgeführt werden können. Bei der Erstellung des Images werden die Projekte kompiliert und die benötigte Laufzeitumgebung installiert. |
| `language_model.sln` | Definiert eine Projektmappe, durch die mehrere Projekte zeitgleich in einer Instanz einer IDE wie bspw. Visual Studio geöffnet werden können. Diese Datei ist nicht zwingend erforderlich, vereinfacht aber bei vielen IDEs die Arbeit. |
| `README.md` | Dieses Readme. Enthält Dokumentation zu dem Projekt. |
