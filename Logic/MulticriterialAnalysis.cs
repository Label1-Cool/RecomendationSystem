using DataLayer;
using Logic.Analysis;
using Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class MulticriterialAnalysis
    {
        /// <summary>
        /// Строит множество Парето из заданного множества
        /// </summary>
        /// <param name="educationLinesAndRequirementTable">Множество, из которого требуется строить мн-во Парето</param>
        /// <returns>Построенное множество Парето</returns>
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

        /// <summary>
        /// Вычисяет коэффициент относительной важности для 2-х критериев(i-й и j-й)
        /// </summary>
        /// <param name="wi">Сколько единиц ЛПР хочет получить в i-м критерии</param>
        /// <param name="wj">Сколькими единиц ЛПР готово пожертвовать в j-м критерии</param>
        /// <returns>Коэффициент относительной важности</returns>
        public double GetTeta(int wi, int wj)
        {
            var teta = wj / (wi + wj);
            if (0 < teta && teta < 1)
                return teta;
            else 
                throw new ArithmeticException("Значение коэффициента относительной важности не лежит в пределах 0<teta<1");
        }
    }

    public class Paretto
    {
        /// <summary>
        /// Получает иноформацию из бд и приводит ее к матричному виду для дальнейшей обработки алгоритмом построения мн-ва Парето.
        /// В данной версии считается, что порядок названий в бд меняться на протяжении работы не будет(так по идее и должно быть)
        /// </summary>
        private void CalculateInfoForParretoSet()
        {
            List<EducationLineAndRequirementRow> educationLinesAndRequirementTable;
            using (var context = new RecomendationSystemModelContainer())
            {
                //TODO: Направлению обучения не хватает названия(одного когда не достаточно)

                //Получаем списки всех нарпавлений обучения + информация о требованиях
                educationLinesAndRequirementTable =
                    (from edLines in context.DepartmentEducationLines
                     where edLines.DepartmentLinesRequirement.Count == 3//пока захардкодим(Должно быть обязательно указано 3 экзамена егэ (мин.баллы)
                     select new EducationLineAndRequirementRow()
                     {
                         Id = edLines.Id,
                         Code = edLines.Code,
                         Requirements = (from requirement in edLines.DepartmentLinesRequirement
                                         select requirement.Requirement).ToList()
                     }).ToList();
                //Получаем остальные списки: оценки, увлечения, предпочтения
            }
            if (educationLinesAndRequirementTable.Count > 0)
            {
                //Дальше алгоритм построения множества Паретто
                MulticriterialAnalysis multicriterialAnalysis = new MulticriterialAnalysis();
                var parretoSet = multicriterialAnalysis.ParretoSetCreate(educationLinesAndRequirementTable);
                //Теперь необходимо сузить наше множество:
                #region 1. Выявлении информации об относительной важности критериев
                //Опрос ЛПР
                //TODO: Подумать над возможностью циклического уточнения, с опросом уточяющей информации на каждом шаге
                //+ проверке непротиворечивости нескольких утверждений
                #endregion
                #region 2. Замена менее важных криетриев новыми
                //Пока сделал 1 шаг с фиксированными коэффициентами(нужно обдумать что они правильные). ЛПР в данном случае -мы(я)
                //Пусть у нас 3 критерия  - минимальные баллы по предметам
                //На сколько мы готовы пожертвовать русским ради математикик и профильного предмета(информатика)

                //когда один критерий важнее второго, а он, в свою очередь,
                //важнее третьего (здесь можно дважды применить теорему 2.5 —
                //сначала пересчитывается третий критерий, а затем второй; см.
                //п. 1 разд. 4.1)
                //Пусть русский<математика<информатика
                //      80      70 
                //      30      60
                //      60      30
                //Пересчитываем сначала русский, потом математику
                //      72
                //      72
                //      72
                //...
                double tetaMath_Rus = multicriterialAnalysis.GetTeta(20, 10);
                double tetaProf_Math = multicriterialAnalysis.GetTeta(20, 5);
                //Определить на каких позициях стоит какой предмет и пересчить поп формуле. Пусть для начала считаем что на 0 - русский, на 1 - математика, на 3-информатика
                foreach (var row in parretoSet)
                {
                    //teta*болееВажный + (1-teta)* менееВажный
                    //Пересчитываем русский 70 80 -> 72 80
                    //Пересчитываем информатику 80 90-> .. ..
                    //...
                    for (int i = 0; i < row.Requirements.Count - 1; i++)
                    {
                        row.Requirements[i] = RecalculateElementsRow(row.Requirements[i + 1], row.Requirements[i], tetaMath_Rus);
                    }
                }
                //или
                //когда два критерия по отдельности важнее третьего (тогда для формирования 
                //нового векторного критерия следует использовать формулу (4.7)

                //или еще варианты...


                //TODO: Решить как распределять коэффициенты и какой у нас случай. По идее мы можем сгруппировать наши требования в группы: ЕГЭ, Оценки, Увлечения и пр.
                //В каждой из них - есть свои требования(балы по математике, русскому, информтатике) которые так же сравнимы... как быть. Счить все как 1 кучу и выявять нужные или по группам.
                //Есть идея сделать рекурсивно:
                //Сначала считаем на вершинах дерева - сравниваем Егэ по русскому, математике и информатике. Получим равнозначные критерии в итоге.
                //Потом сравниваем полученную группу из 3-х критериев с остальными.

                //Самый простой вариант на мой взгялд - заранее определить важность в виде a1<a2<a3<a4<...<an
                //После этого упорядочить наш список критериев(в бд или в коде)
                #endregion
                #region Нахождение нового мноежства паретто
                var parretoSetNew = multicriterialAnalysis.ParretoSetCreate(parretoSet);
                #endregion
                #region 3. Анализ ЛПР-ом получившихся значений

                #endregion

            }
            else
                throw new Exception("Не удалось получить списко информации об направлениях обучения и требованиях");

        }
        private int RecalculateElementsRow(int ImpotantElement, int notImpotantElement, double teta)
        {
            return (int)(teta * ImpotantElement + (1 - teta) * notImpotantElement);
        }
    }
}
