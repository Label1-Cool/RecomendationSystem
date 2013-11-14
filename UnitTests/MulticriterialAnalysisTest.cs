using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Logic;

namespace UnitTests
{
    [TestClass]
    public class MulticriterialAnalysisTest
    {
        //TODO: Написать еще методов тестирования с другими таблицами
        [TestMethod]
        public void Test_ParretoSetCreate_ExistTable()
        {
            var y1=new List<int>();
            y1.AddRange(new int[]{4,0,3,2});
            var y2=new List<int>();
            y2.AddRange(new int[]{5,0,2,2});
            var y3=new List<int>();
            y3.AddRange(new int[]{2,1,1,3});
            var y4=new List<int>();
            y4.AddRange(new int[]{5,0,1,2});
            var y5=new List<int>();
            y5.AddRange(new int[]{3,1,2,3});
            
            List<EducationLineAndRequirementRow> table = new List<EducationLineAndRequirementRow>();
            table.Add(new EducationLineAndRequirementRow
                {
                    Code = "y1",
                    Id = 1,
                    Requirements = y1
                });
            table.Add(new EducationLineAndRequirementRow
            {
                Code = "y2",
                Id = 2,
                Requirements = y2
            });
            table.Add(new EducationLineAndRequirementRow
            {
                Code = "y3",
                Id = 3,
                Requirements = y3
            });
            table.Add(new EducationLineAndRequirementRow
            {
                Code = "y4",
                Id = 4,
                Requirements = y4
            });
            table.Add(new EducationLineAndRequirementRow
            {
                Code = "y5",
                Id = 5,
                Requirements = y5
            });
            
            var answerTable = new List<EducationLineAndRequirementRow>();
            answerTable.Add(table[0]);
            answerTable.Add(table[1]);
            answerTable.Add(table[4]);

            MulticriterialAnalysis multicriterialAnalysis = new MulticriterialAnalysis();
            var parretoTable = multicriterialAnalysis.ParretoSetCreate(table);

            if (parretoTable.Count == answerTable.Count)
                for (int i = 0; i < answerTable.Count; i++)
                {
                    if (parretoTable[i] != answerTable[i])
                    {
                        Assert.Fail("Элемент таблицы не совпадает с ответом.");
                        break;
                    }
                }
            else Assert.Fail("Не совпадает количество элеметнов в матрице ответа и полученной матрице.");
        }
    }
}
