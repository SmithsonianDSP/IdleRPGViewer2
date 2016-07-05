using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Linq;

namespace IdleRPGViewer2
{
    public static class GetEventRows
    {
        public static List<EventRow> GetData(string user = "SmithsonianDSP", int hoursHistory = 6, int eventTypes = -1)
        {
            using (SqlConnection connection = new SqlConnection(DbConnStr.cstring))
            {
                var dtable = new DataTable();
                var eventRows = new List<EventRow>();


                    var dbSelect = new SqlCommand();

                    var selectString = new StringBuilder();

                    selectString.Append("SELECT [t0].[Id], [t0].[UserName], [t0].[EventText], [t0].[QuestLevel], [t0].[EventType] FROM [dbo].[Events] AS [t0]");
                    selectString.Append(" WHERE ([t0].[Id] >= @p0)");

                    dbSelect.Parameters.Add(new SqlParameter("@p0", DateTimeOffset.Now.AddHours(-1 * hoursHistory)));

                    if (user != "*")
                    {
                        selectString.Append("  AND ([t0].[UserName] = @p1)");
                        dbSelect.Parameters.Add(new SqlParameter("@p1", user));
                    }

                    if (eventTypes != -1)
                    {
                        selectString.Append(" AND ([t0].[EventType] = @p2)");
                        dbSelect.Parameters.Add(new SqlParameter("@p2", eventTypes));
                    }

                    var selectCommandText = selectString.ToString();

                    dbSelect.CommandText = selectCommandText;
                    dbSelect.CommandType = CommandType.Text;
                    dbSelect.Connection = connection;

                try
                {
                    connection.Open();
                    var tAdapter = new SqlDataAdapter(dbSelect);
                    tAdapter.Fill(dtable);
                    connection.Close();
                }

                catch (Exception ex)
                {
                    eventRows.Add(new EventRow { EventText = ex.Message });
                    return eventRows;
                }

                foreach (DataRow rw in dtable.Rows)
                {
                    try
                    {
                        eventRows.Add(new EventRow
                        {
                            EventDate = DateTimeOffset.Parse(rw[0].ToString()),
                            User = rw[1].ToString(),
                            EventText = rw[2].ToString().Replace("*", ""),
                            QLevel = int.Parse(rw[3].ToString()),
                            EventType = int.Parse(rw[4].ToString())
                        });
                    }
                    catch (Exception ex)
                    {
                        eventRows.Add(new EventRow { EventText = ex.Message, EventType = -1 });
                    }
                }

                return eventRows.DistinctBy(e => e.EventText).ToList();
            }
        }


        public static List<EventRow> GetCheckerEvents()
        {
            // set default query params for background check queries
            int eventTypes = 1;
            string user = "SmithsonianDSP";

            var queryDateRange = CheckerJobService.GetLastCheckedDateTime();

            if (queryDateRange <= DateTimeOffset.Now.AddHours(-6))
            {
                queryDateRange = DateTimeOffset.Now.AddHours(-1);
            }

            using (SqlConnection connection = new SqlConnection(DbConnStr.cstring))
            {
                var dtable = new DataTable();
                var eventRows = new List<EventRow>();

                var dbSelect = new SqlCommand();
                var selectString = new StringBuilder();

                selectString.Append("SELECT [t0].[Id], [t0].[UserName], [t0].[EventText], [t0].[QuestLevel], [t0].[EventType] FROM [dbo].[Events] AS [t0]");

                selectString.Append(" WHERE ([t0].[Id] >= @p0)");
                dbSelect.Parameters.Add(new SqlParameter("@p0", queryDateRange));

                selectString.Append("  AND ([t0].[UserName] = @p1)");
                dbSelect.Parameters.Add(new SqlParameter("@p1", user));

                selectString.Append(" AND ([t0].[EventType] = @p2)");
                dbSelect.Parameters.Add(new SqlParameter("@p2", eventTypes));

                var selectCommandText = selectString.ToString();

                dbSelect.CommandText = selectCommandText;
                dbSelect.CommandType = CommandType.Text;
                dbSelect.Connection = connection;

                try
                {
                    connection.Open();
                    var tAdapter = new SqlDataAdapter(dbSelect);
                    tAdapter.Fill(dtable);
                    connection.Close();
                }
                catch (Exception)
                {
                    return null;
                }


                foreach (DataRow rw in dtable.Rows)
                {
                    try
                    {
                        eventRows.Add(new EventRow
                        {
                            EventDate = DateTimeOffset.Parse(rw[0].ToString()),
                            User = rw[1].ToString(),
                            EventText = rw[2].ToString().Replace("*", ""),
                            QLevel = int.Parse(rw[3].ToString()),
                            EventType = int.Parse(rw[4].ToString())
                        });
                    }
                    catch (Exception ex)
                    {
                        eventRows.Add(new EventRow { EventText = ex.Message, EventType = -1 });
                    }
                }
                return eventRows.DistinctBy(e => e.EventText).ToList();
            }
        }



    }
    public class EventRow
    {
        public DateTimeOffset EventDate { get; set; }
        public int EventType { get; set; }
        public int QLevel { get; set; }
        public string User { get; set; }
        public string EventText { get; set; }
    }
}
public static class GroupByExt
{
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> knownKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (knownKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}


