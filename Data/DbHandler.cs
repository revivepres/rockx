using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace rockx.Data
{
    public class DbHandler
    {
        private SqlConnection _connection;

        public DbHandler(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public async Task<List<Person>> GetPeople()
        {
            await Task.Delay(10);
            var mockdata = new MockData();
            try
            {
                //await _connection.OpenAsync();
                //var command = new SqlCommand($"SELECT * FROM {_tablename} WHERE id = {item.Id}", _connection);
                //var reader = await command.ExecuteReaderAsync();
                //if (reader.FieldCount > 0) { }
                //_connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return mockdata.People();
        }

        public async Task<List<DateTime>> GetDates()
        {
            await Task.Delay(10);
            var mockdata = new MockData();
            return mockdata.Dates();
        }

        public async Task<List<Person>> GetPeopleForDate(DateTime date)
        {
            await Task.Delay(10);
            var mockdata = new MockData();
            try
            {
                //await _connection.OpenAsync();
                //var command = new SqlCommand($"SELECT * FROM {_tablename} WHERE id = {item.Id}", _connection);
                //var reader = await command.ExecuteReaderAsync();
                //if (reader.FieldCount > 0) { }
                //_connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return mockdata.People();
        }

        public async Task<int> GetGuestsForDate(DateTime date)
        {
            await Task.Delay(10);
            var mockdata = new MockData();
            try
            {
                //await _connection.OpenAsync();
                //var command = new SqlCommand($"SELECT * FROM {_tablename} WHERE id = {item.Id}", _connection);
                //var reader = await command.ExecuteReaderAsync();
                //if (reader.FieldCount > 0) { }
                //_connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return 7;
        }

        public async Task AddAttendance(Attendance attendance)
        {
            await Task.Delay(10);
            try
            {
                //await _connection.OpenAsync();
                //SqlCommand cmd = new SqlCommand($"INSERT INTO {_tablename} ([id],[column1],[column2],[column3],[column4],[column5],[column6],[column7],[column8],[column9]) " +
                //    "VALUES (@id,@column1,@column2,@column3,@column4,@column5,@column6,@column7,@column8,@column9)", _connection);
                //cmd.Parameters.Add(new SqlParameter("@id", item.Id));
                //cmd.Parameters.Add(new SqlParameter("@column1", item.Column1));
                //cmd.Parameters.Add(new SqlParameter("@column2", item.Column2));
                //cmd.Parameters.Add(new SqlParameter("@column3", item.Column3));
                //cmd.Parameters.Add(new SqlParameter("@column4", item.Column4));
                //cmd.Parameters.Add(new SqlParameter("@column5", item.Column5));
                //cmd.Parameters.Add(new SqlParameter("@column6", item.Column6));
                //cmd.Parameters.Add(new SqlParameter("@column7", item.Column7));
                //cmd.Parameters.Add(new SqlParameter("@column8", item.Column8));
                //cmd.Parameters.Add(new SqlParameter("@column9", item.Column9));
                //await cmd.ExecuteNonQueryAsync();
                //_connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return;
        }

        public async Task AddGuest(Guests guests)
        {
            await Task.Delay(10);
            try
            {
                //await _connection.OpenAsync();
                //SqlCommand cmd = new SqlCommand($"INSERT INTO {_tablename} ([id],[column1],[column2],[column3],[column4],[column5],[column6],[column7],[column8],[column9]) " +
                //    "VALUES (@id,@column1,@column2,@column3,@column4,@column5,@column6,@column7,@column8,@column9)", _connection);
                //cmd.Parameters.Add(new SqlParameter("@id", item.Id));
                //cmd.Parameters.Add(new SqlParameter("@column1", item.Column1));
                //cmd.Parameters.Add(new SqlParameter("@column2", item.Column2));
                //cmd.Parameters.Add(new SqlParameter("@column3", item.Column3));
                //cmd.Parameters.Add(new SqlParameter("@column4", item.Column4));
                //cmd.Parameters.Add(new SqlParameter("@column5", item.Column5));
                //cmd.Parameters.Add(new SqlParameter("@column6", item.Column6));
                //cmd.Parameters.Add(new SqlParameter("@column7", item.Column7));
                //cmd.Parameters.Add(new SqlParameter("@column8", item.Column8));
                //cmd.Parameters.Add(new SqlParameter("@column9", item.Column9));
                //await cmd.ExecuteNonQueryAsync();
                //_connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return;
        }

    }
}
