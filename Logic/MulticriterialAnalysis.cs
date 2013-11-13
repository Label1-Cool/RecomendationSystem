﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class MulticriterialAnalysis
    {
        public List<EducationLineAndRequirementRow> ParretoSetCreate(List<EducationLineAndRequirementRow> educationLinesAndRequirementTable)
        {
            List<EducationLineAndRequirementRow> parretoList = new List<EducationLineAndRequirementRow>();
            //Тут 2 варианта:
            //1. Получаем 1 общий список со всеми прараметрами и строим мн-во Парето(оценки, предпочтения, пр)
            //2. Или несколько, и тогда находим паретто для каждого и ищем пересечение
            //(Пока делаем только 1-й)
            for (int i = 0; i < educationLinesAndRequirementTable.Count; i++)
            {
                var comparedItemRequirements = educationLinesAndRequirementTable[i].Requirements.ToArray();
                for (int j = i + 1; j < educationLinesAndRequirementTable.Count; j++)
                {
                    var itemRequirements = educationLinesAndRequirementTable[j].Requirements.ToArray();
                    bool comparedIsMore = true;
                    for (int k = 0; k < comparedItemRequirements.Count(); k++)
                    {
                        if (comparedItemRequirements[k] < itemRequirements[k])
                        {
                            //переход к шагу 5 алгоритма
                            comparedIsMore = false;
                            break;
                        }
                    }

                    if (comparedIsMore)
                    {
                        //1-й вектор оказался больше 2-го, поэтому удаляем 2-й из общего множества
                        educationLinesAndRequirementTable.Remove(educationLinesAndRequirementTable[j]);
                        //т.к. массив по которому мы проходим поменялся
                        --j;
                    }
                    else//проверяем на равенство эти же объекты, но в другом порядке(шаг 5)
                    {
                        bool itemIsMore = true;
                        for (int k = 0; k < comparedItemRequirements.Count(); k++)
                        {
                            if (itemRequirements[k] < comparedItemRequirements[k])
                            {
                                itemIsMore = false;
                                break;
                            }
                        }
                        if (itemIsMore)
                        {
                            //2-й вектор оказался больше 1-го, поэтому удаляем 1-й из общего множества
                            educationLinesAndRequirementTable.Remove(educationLinesAndRequirementTable[i]);
                            //т.к. массив по которому мы проходим поменялся
                            --i;break;
                        }
                    }

                }
                //После выполнения сравнения со всеми векторами, если вектор не удалился в процессе - удаляем его
                if (i!=-1)
                {
                    parretoList.Add(educationLinesAndRequirementTable[i]);
                    educationLinesAndRequirementTable.Remove(educationLinesAndRequirementTable[0]);
                    --i;
                }
            }

            return parretoList;
        }
    }
}