# Aufgabe 2 - Language Model

| Gruppenmitglieder |
|-|
| Robin Augenstein |
| Kai Petelski |
| Jannick Gutekunst |

## TOC

- [Aufgabe 2 - Language Model](#aufgabe-2---language-model)
  - [TOC](#toc)
  - [Lernen eines Language-Models](#lernen-eines-language-models)
  - [Berechnung der Perplexität eines Eingabesatzes](#berechnung-der-perplexität-eines-eingabesatzes)
  - [Struktur des Repositories](#struktur-des-repositories)
  - [Bauen der Programme](#bauen-der-programme)
  - [Starten der Programme](#starten-der-programme)

## Lernen eines Language-Models

- Das Program akzeptiert verschiedene Parameter, die bei dem Start von ebendiesem übergeben werden können. Auf diese Weise erhält das Programm alle benötigten Informationen, beispielsweise den Pfad zu dem Eingabetext, der zu verwendenden Smoothing-Variante und dem Ausgabepfad. Diese werden in einer Instanz der Klasse `LearnOptions` (siehe `Learn/LearnOptions.cs`) gespeichert.
- Anschließend wird in der Funktion `Learn()` der Klasse `Program` (siehe `Learn/Program.cs`) eine Instanz der Klasse `LanguageModelLearner` (siehe `LanguageModel/LanguageModellearner`) erstellt. Diese Klasse besitzt eine  interne Datenstruktur sowie Funktionen, die es ermöglichen, die in einem Text vorkommenden N-Gramme zu zählen und hieraus ein Language Model zu generieren. Desweiteren wird eine Instanz der Klasse `StreamReader` erstellt, die es ermöglicht, Text aus der angegebenen Eingabedatei zu lesen. Hierbei werden über einen `try {...} catch (...) {...}`-Mechanismus auftretende Fehler behandelt und an den Nutzer kommuniziert. Die Ausgabe dieser Fehler an den Nutzer ist an die Methode `HandleExceptions()` delegiert, um die Duplizierung von weitestgehend identischem Code soweit wie möglich zu vermeiden.
- Nach diesen Vorbereitungen werden durch den Aufruf der Methode `Learn` auf die zuvor erstellte Instanz der Klasse `LanguageModelLearner` die in der Eingabedatei vorkommenden N-Gramme gezählt. Diese Daten werden in der internen Datenstruktur der Instanz zwischengespeichert.
- Nachdem die N-Gramme im Lerntext gezählt wurden, wird aus deren Auftrittshäufigkeiten in dem Aufruf `BuildLanguageModel(Smoothing)` ein Language Model erzeugt. Die wesentliche Aktion, die hierbei durchgeführt wird, ist die Berechnung der N-Gram Wahrscheinlichkeiten unter Anwendung des bei dem Aufruf angegebenen Smoothing-Verfahrens. Als Ergebnis wird von dieser Funktion eine Instanz der Klasse `NGramLanguageModel` zurückgegeben, die intern in einer Datenstruktur die N-Gramme und deren Wahrscheinlichkeiten enthält.
- Abschließend wird auf dieses neue Objekt die Funktion `GetArpaRepresentation` aufgerufen, wodurch die textbasierte ARPA-Darstellung dieses Language Models in einem `Stream` zwischengespeichert wird. Diese Zwischenspeicherung bietet gegenüber dem direkten Schreiben auf den Datenträger den Vorteil, dass zum einen die Datei auf einmal und nicht in kleinen Stücken geschrieben wird, wodurch die Anzahl der erforderlichen Datenträgerzugriffe minimiert und somit auch die Ausführungszeit gesenkt werden kann. Desweiteren können die zwischengespeicherten Daten auch noch weiterverwendet werden, um beispielsweise die ARPA-Darstellung zusätzlich auf der Konsolenoberfläche auszugeben ohne dass die Daten zunächst von dem Datenträger gelesen werden müssen.
- Als letztes wird die bei Programmstart angegebene Ausgabedatei mittels `File.OpenWrite` schreibend geöffnet und die ARPA-Darstellung des Language Models in diese geschrieben.

## Berechnung der Perplexität eines Eingabesatzes

## Struktur des Repositories

| Verzeichnis | Inhalt |
|-|-|
| `.vscode/` | Enthält erforderliche Metadaten für die Verwendung von VS Code als IDE. |
| `doc/img/` | Enthält Bilder, die in der Dokumentation angezeigt werden. |
| `LanguageModel/` | Enthält eine selbsterstellte Bibliothek, die Funktionen, die von diesen beiden Programmen genutzt werden, bereitstellt. |
| `LanguageModel/Smoothing/` | Enthält die Implementierung von verschiedenen Smoothing-Verfahren, die bei dem Lernen eines Language Models eingesetzt werden können. Aktuell sind Regular-Smoothing und Kneser-Ney-Smoothing implementiert. |
| `Learn/` | Enthält das Programm, mit dem ein Language Model aus einem Eingabetext gelernt werden kann. |
| `Perplexity/` | Enthält das Programm, mit dem die Perplexität eines Eingabesatzes unter Verwendung eines im ARPA-Format bereitgestellten Language Models bestimmt werden kann. |
| `.gitignore` | gitignore-Datei - definiert, welche Dateien nicht von Git versioniert werden sollen. |
| `Dockerfile` | Ermöglicht die automatisierte Erstellung eines Docker-Images, in dem die Programme direkt ausgeführt werden können. Bei der Erstellung des Images werden die Projekte kompiliert und die benötigte Laufzeitumgebung installiert. |
| `language_model.sln` | Definiert eine Projektmappe, durch die mehrere Projekte zeitgleich in einer Instanz einer IDE wie bspw. Visual Studio geöffnet werden können. Diese Datei ist nicht zwingend erforderlich, vereinfacht aber bei vielen IDEs die Arbeit. |
| `README.md` | Dieses Readme. Enthält Dokumentation zu dem Projekt. |

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
