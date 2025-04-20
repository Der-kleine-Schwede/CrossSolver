using System.Xml;
using CrossSolver.API;
using CrossSolver.API.Data;
using HtmlAgilityPack;
using static System.Net.WebRequestMethods;

public static partial class ProblemSolver 
{
    /// <summary>
    /// Erstellt eine Webanfrage mit der übergebenenen URL und liefert die Website als HTML.
    /// </summary>
    static HtmlDocument GetDocument(string url) {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        return doc;
    }

    /// <summary>
    /// ToDo: Diese Klasse als Abstrakte Klasse umbauen und Methoden auslagern in eine abgeleitete Klasse
    /// </summary>
    class Problem {
        public string question = "";
        public string answer = "";

        public Problem(string question, string answer) {
            this.question = question;
            this.answer = answer;
        }

        public Problem() { }

        /// <returns>True für: "Anfrage war erfolgreich; Zur Frage wurde eine Antwort der passenden Länge gefunden</returns>
        public bool IsDefined() {
            return question != "" && answer != "";
        }
    };

    /// <summary>
    /// Sucht anhand der Parameter auf der Website www.kreuzwort-raetsel.net nach einer passenden Lösung
    /// ToDo: Diese Methode gibt derzeit noch eine Liste von Lösungen zurück. Hier muss eine einzige Lösung aus der Liste ausgewählt werden, die am besten passt.
    /// Außerdem sollten alle neuen Lösung in eine Datenbank eingetragen werden.
    /// 
    /// Siehe wiki zu dieser Klasse ProblemSolver für weitere Information.
    /// </summary>
    /// <param name="question">String von mindestens einem Wort</param>
    /// <param name="expectedLength">Erwartete Länge der Lösung</param>
    public static string Solve(string question, int expectedLength) 
    {

        string formatedQuestion = question.Trim();
        //Wird genau für diese URL formatiert; Muss für andere Websiten entsprechend angepasst werden.
        formatedQuestion = formatedQuestion.Replace(" ", "+");

        string AnswersSourceURL = $"https://www.kreuzwort-raetsel.net/suche.php?s={formatedQuestion}&field={expectedLength}&go=suchen";

        string formatedURL = string.Format(AnswersSourceURL, formatedQuestion, expectedLength);

        HtmlDocument document = GetDocument(formatedURL);
        //Console.WriteLine(document.DocumentNode.OuterHtml);

        /*problems ist eine Liste aller auf der Website gefundenen Fragen und dazugehöriger Antworten.*/
        List<Problem> problems = new List<Problem>();

        string result = "";

        //HTML Dokument parsen. ToDo: Sollte in eine Parse Html Klasse ausgelagert werden; nach folgender Signatur public Parse(String) liefert passendes HtmlDocument Unterklasse
        //Eventuell gibt es dazu von HtmlDocument schon eine passende Methode
        foreach (HtmlNode node in document.DocumentNode.Descendants()) {

            if (node.Name == "tr" && node.InnerHtml.ToString().Contains("href=\"frage")) {
                //Auf der Website werden mehrere Antworten angezeigt die zu der Frage passen könnten. Alle diese werden erstmal in der Liste problems hinzugefügt.
                foreach(HtmlNode questionNode in node.SelectNodes("//td")) 
                {

                    Problem answerToAdd = new Problem();
                    if (questionNode.Name == "td" && questionNode.Attributes["class"].ToString() == "Question") 
                    {
                        answerToAdd.question = questionNode.InnerText;
                    }
                    if (questionNode.Name == "td" && questionNode.Attributes["class"].ToString() == "Answer") {
                        answerToAdd.answer = questionNode.InnerText;
                    }

                    if (answerToAdd.IsDefined()) { 
                        problems.Add(answerToAdd);
                    }
                }
                //ToDo: Lösungen in eine Datenbank eintragen
            }
        }

        //TODO: Hier muss das richtige Element aus List<Problem> problems ausgewählt und zurückgegeben werden; vorzugsweise anhand der expectedLength aus den Parametern.
        return result;
    }
}
