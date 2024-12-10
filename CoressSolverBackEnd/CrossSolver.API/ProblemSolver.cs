using System.Xml;
using HtmlAgilityPack;
using static System.Net.WebRequestMethods;

public static class ProblemSolver 
{

    static HtmlDocument GetDocument(string url) {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        return doc;
    }

    class Answer {
        public string question = "";
        public string answer = "";

        public Answer(string question, string answer) {
            this.question = question;
            this.answer = answer;
        }
    };

    //static DocumentNode Get

    /// <summary>
    /// Sucht anhand der Parameter auf der Website www.kreuzwort-raetsel.net nach einer passenden Lösung
    /// ToDo: Diese Methode gibt derzeit noch eine Liste von Lösungen zurück. Hier muss eine einzige aus der Liste ausgewählt werden, die am besten passt.
    /// Außerdem sollten alle neuen Lösung in eine Datenbank eingetragen werden.
    /// </summary>
    /// <param name="question">String von mindestens einem Wort</param>
    /// <param name="expectedLength">Erwartete Länge der Lösung</param>
    /// <returns></returns>
    public static string Solve(string question, int expectedLength) 
    {

        string formatedQuestion = question.Trim();
        formatedQuestion = formatedQuestion.Replace(" ", "+");

        string AnswersSourceURL = $"https://www.kreuzwort-raetsel.net/suche.php?s={formatedQuestion}&field={expectedLength}&go=suchen";

        string formatedURL = string.Format(AnswersSourceURL, formatedQuestion, expectedLength);

        HtmlDocument document = GetDocument(formatedURL);
        //Console.WriteLine(document.DocumentNode.OuterHtml);

        List<Answer> Answers = new List<Answer>();

        List<HtmlNode> nodes = new List<HtmlNode>();
        string result = "";

        foreach (HtmlNode node in document.DocumentNode.Descendants()) {

            if (node.Name == "tr" && node.InnerHtml.ToString().Contains("href=\"frage")) {
                //Hier muss eine Auswahl getroffen werden und nur die passenste Antwort zurückgegeben werden
                result += node.InnerHtml.ToString();
                nodes.Add(node);

                //ToDo: Lösungen in eine Datenbank eintragen
            }
        }
        return result;
    }
}
