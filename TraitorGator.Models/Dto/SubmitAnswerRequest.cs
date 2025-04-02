using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitorGator.Models.Dto
{
    public class SubmitAnswerRequest
    {
        public Guid PlayerId { get; set; }
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
}
