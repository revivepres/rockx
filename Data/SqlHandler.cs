using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace rockx.Data
{
    public class SqlHandler : IDbHandler
    {
        private SqlConnection _connection;

        public SqlHandler(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<Person>> GetPeopleFromGroup(int groupId)
        {
            List<Person> list = new List<Person>();
            try
            {
                await _connection.OpenAsync();
                var command = new SqlCommand($@"SELECT person.firstname, person.lastname, personalias.id FROM person JOIN personalias 
                    ON person.id = personalias.personid JOIN groupmember ON person.id = groupmember.personid 
                    WHERE person.issystem = 'false' AND groupmember.groupid = {groupId}
                    ORDER BY person.firstname", _connection);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Person p = new Person();
                    p.FirstName = reader[0].ToString();
                    p.LastName = reader[1].ToString();
                    p.Id = Int32.Parse(reader[2].ToString());
                    list.Add(p);
                }
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return list;
        }

        public async Task<List<DateTime>> GetDates()
        {
            List<DateTime> dates = new List<DateTime>();
            try
            {
                await _connection.OpenAsync();
                var command = new SqlCommand($"SELECT DISTINCT sundaydate FROM attendance WHERE sundaydate > DATEADD(DAY, DATEDIFF(DAY, 365, GETDATE()), 0) ORDER BY sundaydate DESC", _connection);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var d = Convert.ToDateTime(reader[0]);
                    dates.Add(d);
                }
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return dates;
        }

        public async Task<List<Person>> GetPeopleForDate(DateTime date)
        {
            List<Person> list = new List<Person>();
            try
            {
                await _connection.OpenAsync();
                var sqlDate = date.ToString("yyyyMMdd");
                var command = new SqlCommand($@"
                    SELECT person.firstname, person.lastname, personalias.id, attendance.didattend
                    FROM attendance JOIN personalias
                    ON attendance.personaliasid = personalias.id
                    JOIN person
                    ON personalias.personid = person.id
                    WHERE attendance.groupid = 24 AND attendance.sundaydate = '{sqlDate}'", _connection);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Person p = new Person();
                    p.FirstName = reader[0].ToString();
                    p.LastName = reader[1].ToString();
                    p.Id = Int32.Parse(reader[2].ToString());
                    p.IsAttend = (bool)reader[3];
                    list.Add(p);
                }
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return list;
        }

        public async Task<int> GetGuestsForDate(DateTime date)
        {
            int guestCount = 0;
            try
            {
                await _connection.OpenAsync();
                var sqlDate = date.ToString("yyyyMMdd");
                var command = new SqlCommand($@"
                    SELECT count FROM attendanceguests
                    WHERE sundaydatetime =  '{sqlDate}'
                    ORDER BY createddatetime DESC", _connection);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    guestCount = Int32.Parse(reader[0].ToString());
                    break;
                }
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return guestCount;
        }

        public async Task AddAttendance(List<Attendance> attendance, int guestCount)
        {
            try
            {
                await _connection.OpenAsync();
                // Some initial values
                var sundaydate = attendance[0].StartDateTime;
                var createddate = attendance[0].CreatedDateTime;
                var createdby = attendance[0].CreatedByPersonAliasId;

                // Clear out the tables
                SqlCommand cmdDeleteAttendance = new SqlCommand($@"
                    DELETE FROM attendance WHERE sundaydate = @sundaydate", _connection);
                cmdDeleteAttendance.Parameters.Add(new SqlParameter("@sundaydate", sundaydate));
                cmdDeleteAttendance.ExecuteNonQuery();
                SqlCommand cmdDeleteGuests = new SqlCommand($@"
                    DELETE FROM attendanceguests WHERE sundaydatetime = @sundaydatetime", _connection);
                cmdDeleteGuests.Parameters.Add(new SqlParameter("@sundaydatetime", sundaydate));
                cmdDeleteGuests.ExecuteNonQuery();

                // Insert the new records
                SqlCommand cmdInsertGuests = new SqlCommand($@"
                INSERT INTO attendanceguests ([createddatetime],[count],[createdbypersonaliasid],[sundaydatetime])
                    VALUES (@createddatetime,@count,@createdbypersonaliasid,@sundaydatetime)", _connection);
                cmdInsertGuests.Parameters.Add(new SqlParameter("@createddatetime", createddate));
                cmdInsertGuests.Parameters.Add(new SqlParameter("@count", guestCount));
                cmdInsertGuests.Parameters.Add(new SqlParameter("@createdbypersonaliasid", createdby));
                cmdInsertGuests.Parameters.Add(new SqlParameter("@sundaydatetime", sundaydate));
                cmdInsertGuests.ExecuteNonQuery();
                foreach (var attendee in attendance)
                {
                    SqlCommand cmd = new SqlCommand($@"
                    INSERT INTO attendance ([locationid],[groupid],[scheduleid],[startdatetime],[didattend],[note],[guid],[createddatetime],[modifieddatetime],
                        [createdbypersonaliasid],[modifiedbypersonaliasid],[campusid],[personaliasid],[rsvp])
                    VALUES (@locationid,@groupid,@scheduleid,@startdatetime,@didattend,@note,@guid,@createddatetime,@modifieddatetime,
                        @createdbypersonaliasid,@modifiedbypersonaliasid,@campusid,@personaliasid,@rsvp)", _connection);
                    cmd.Parameters.Add(new SqlParameter("@locationid", attendee.LocationId));
                    cmd.Parameters.Add(new SqlParameter("@groupid", attendee.GroupId));
                    cmd.Parameters.Add(new SqlParameter("@scheduleid", attendee.ScheduleId));
                    cmd.Parameters.Add(new SqlParameter("@startdatetime", attendee.StartDateTime));
                    cmd.Parameters.Add(new SqlParameter("@didattend", attendee.DidAttend));
                    cmd.Parameters.Add(new SqlParameter("@note", attendee.Note));
                    cmd.Parameters.Add(new SqlParameter("@guid", attendee.Guid));
                    cmd.Parameters.Add(new SqlParameter("@createddatetime", attendee.CreatedDateTime));
                    cmd.Parameters.Add(new SqlParameter("@modifieddatetime", attendee.ModifiedDateTime));
                    cmd.Parameters.Add(new SqlParameter("@createdbypersonaliasid", attendee.CreatedByPersonAliasId));
                    cmd.Parameters.Add(new SqlParameter("@modifiedbypersonaliasid", attendee.ModifiedByPersonAliasId));
                    cmd.Parameters.Add(new SqlParameter("@campusid", attendee.CampusId));
                    cmd.Parameters.Add(new SqlParameter("@personaliasid", attendee.PersonAliasId));
                    cmd.Parameters.Add(new SqlParameter("@rsvp", attendee.Rsvp));
                    cmd.ExecuteNonQuery();
                }
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR - {e.Message}");
            }
            return;
        }
    }
}
