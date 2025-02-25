using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CrossSolver.API {
    [PrimaryKey(propertyName:"Id")]
    public class Answer {
        public int Id { get; set; }
        public string question { get; set; } = "";
        public string answer { get; set; } = "";

        public Answer(string question, string answer) {
            this.question = question;
            this.answer = answer;
        }
    }
};
