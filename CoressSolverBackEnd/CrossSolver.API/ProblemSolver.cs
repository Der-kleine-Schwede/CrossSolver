using System.Xml;
using CrossSolver.API;
using CrossSolver.API.Data;
using HtmlAgilityPack;
using static System.Net.WebRequestMethods;

public static partial class ProblemSolver 
{

    static HtmlDocument GetDocument(string url) {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        return doc;
    }

    class Problem {
        public string question = "";
        public string answer = "";

        public Problem(string question, string answer) {
            this.question = question;
            this.answer = answer;
        }

        public Problem() { }

        public bool IsDefined() {
            return question != "" && answer != "";
        }
    };

    /// <summary>
    /// Sucht anhand der Parameter auf der Website www.kreuzwort-raetsel.net nach einer passenden Lösung
    /// ToDo: Diese Methode gibt derzeit noch eine Liste von Lösungen zurück. Hier muss eine einzige aus der Liste ausgewählt werden, die am besten passt.
    /// Außerdem sollten alle neuen Lösung in eine Datenbank eingetragen werden.
    /// 
    /// Siehe wiki zu dieser Klasse ProblemSolver für weitere Information.
    /// </summary>
    /// <param name="question">String von mindestens einem Wort</param>
    /// <param name="expectedLength">Erwartete Länge der Lösung</param>
    public static string Solve(string question, int expectedLength, CrossSolverAPIContext context) 
    {

        string formatedQuestion = question.Trim();
        formatedQuestion = formatedQuestion.Replace(" ", "+");

        string AnswersSourceURL = $"https://www.kreuzwort-raetsel.net/suche.php?s={formatedQuestion}&field={expectedLength}&go=suchen";

        string formatedURL = string.Format(AnswersSourceURL, formatedQuestion, expectedLength);

        HtmlDocument document = GetDocument(formatedURL);
        //Console.WriteLine(document.DocumentNode.OuterHtml);

        /*problems ist eine Liste aller auf der Website gefundenen Fragen und dazugehöriger Antworten.*/
        List<Problem> problems = new List<Problem>();

        string result = "";

        foreach (HtmlNode node in document.DocumentNode.Descendants()) {

            if (node.Name == "tr" && node.InnerHtml.ToString().Contains("href=\"frage")) {
                //Hier muss eine Auswahl getroffen werden und nur die passenste Antwort zurückgegeben werden
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
