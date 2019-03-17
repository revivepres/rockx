using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
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
                var command = new SqlCommand($@"
                    SELECT person.firstname, person.lastname, personalias.id FROM person 
                    JOIN personalias 
                    ON person.id = personalias.personid 
                    JOIN groupmember ON person.id = groupmember.personid 
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
                var command = new SqlCommand($"SELECT DISTINCT sundaydate FROM attendanceoccurrence WHERE sundaydate > DATEADD(DAY, DATEDIFF(DAY, 365, GETDATE()), 0) ORDER BY sundaydate DESC", _connection);
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

        public async Task<List<Person>> GetPeopleFromGroupByDate(int groupId, DateTime date)
        {
            List<Person> list = new List<Person>();
            try
            {
                await _connection.OpenAsync();
                var sqlDate = date.ToString("yyyyMMdd");
                var command = new SqlCommand($@"
                    SELECT person.firstname, person.lastname, personalias.id, attendance.didattend
                    FROM attendance 
                    JOIN personalias
                    ON attendance.personaliasid = personalias.id
                    JOIN person
                    ON personalias.personid = person.id
                    JOIN attendanceoccurrence
                    ON attendanceoccurrence.id = attendance.occurrenceid
                    WHERE attendanceoccurrence.groupid = {groupId} AND attendanceoccurrence.sundaydate = '{sqlDate}'
                    ORDER BY person.firstname", _connection);
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

                // Clear out the tables if replacing record
                SqlCommand cmdGetOccurrenceId = new SqlCommand($@"
                    SELECT id FROM attendanceoccurrence WHERE sundaydate = @sundaydate", _connection);
                cmdGetOccurrenceId.Parameters.Add(new SqlParameter("@sundaydate", sundaydate));
                var result = cmdGetOccurrenceId.ExecuteScalar();
                if (result != null)
                {
                    var oid = Convert.ToInt32(result);
                    // Delete from Attendance
                    SqlCommand cmdDeleteAttendance = new SqlCommand($@"
                        DELETE FROM attendance WHERE occurrenceid = @occurrenceid", _connection);
                    cmdDeleteAttendance.Parameters.Add(new SqlParameter("@occurrenceid", oid));
                    cmdDeleteAttendance.ExecuteNonQuery();

                    // Delete from Guest
                    SqlCommand cmdDeleteGuests = new SqlCommand($@"
                        DELETE FROM attendanceguests WHERE occurrenceid = @occurrenceid", _connection);
                    cmdDeleteGuests.Parameters.Add(new SqlParameter("@occurrenceid", oid));
                    cmdDeleteGuests.ExecuteNonQuery();

                    // Delete from Occurrence
                    SqlCommand cmdDeleteOccurrence = new SqlCommand($@"
                        DELETE FROM attendanceoccurrence WHERE id = @id", _connection);
                    cmdDeleteOccurrence.Parameters.Add(new SqlParameter("@id", oid));
                    cmdDeleteOccurrence.ExecuteNonQuery();
                }

                // Add to Occurrence
                SqlCommand cmdInsertOccurrence = new SqlCommand($@"
                INSERT INTO attendanceoccurrence ([groupid],[locationid],[scheduleid],[occurrencedate],[didnotoccur],[guid],[createddatetime],
                    [modifieddatetime],[createdbypersonaliasid],[modifiedbypersonaliasid],[notes],[anonymousattendancecount])
                    OUTPUT INSERTED.ID
                    VALUES (@groupid,@locationid,@scheduleid,@occurrencedate,@didnotoccur,@guid,@createddatetime,@modifieddatetime,@createdbypersonaliasid,
                    @modifiedbypersonaliasid,@notes,@anonymousattendancecount)", _connection);
                cmdInsertOccurrence.Parameters.Add("@id", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@groupid", attendance[0].GroupId));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@locationid", attendance[0].LocationId));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@scheduleid", attendance[0].ScheduleId));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@occurrencedate", sundaydate));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@didnotoccur", false));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@guid", Guid.NewGuid()));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@createddatetime", attendance[0].CreatedDateTime));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@modifieddatetime", attendance[0].ModifiedDateTime));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@createdbypersonaliasid", attendance[0].CreatedByPersonAliasId));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@modifiedbypersonaliasid", attendance[0].ModifiedByPersonAliasId));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@notes", "Added by rockx"));
                cmdInsertOccurrence.Parameters.Add(new SqlParameter("@anonymousattendancecount", guestCount));
                var occurrenceid = cmdInsertOccurrence.ExecuteScalar(); //.Parameters["@id"].Value;
                //cmdInsertOccurrence.ExecuteNonQuery();

                // Add to Attendance
                foreach (var attendee in attendance)
                {
                    SqlCommand cmdInsertAttendance = new SqlCommand($@"
                    INSERT INTO attendance ([startdatetime],[didattend],[note],[guid],[createddatetime],[modifieddatetime],
                        [createdbypersonaliasid],[modifiedbypersonaliasid],[campusid],[personaliasid],[rsvp],[occurrenceid])
                    VALUES (@startdatetime,@didattend,@note,@guid,@createddatetime,@modifieddatetime,
                        @createdbypersonaliasid,@modifiedbypersonaliasid,@campusid,@personaliasid,@rsvp,@occurrenceid)", _connection);
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@startdatetime", attendee.StartDateTime));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@didattend", attendee.DidAttend));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@note", attendee.Note));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@guid", attendee.Guid));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@createddatetime", attendee.CreatedDateTime));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@modifieddatetime", attendee.ModifiedDateTime));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@createdbypersonaliasid", attendee.CreatedByPersonAliasId));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@modifiedbypersonaliasid", attendee.ModifiedByPersonAliasId));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@campusid", attendee.CampusId));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@personaliasid", attendee.PersonAliasId));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@rsvp", attendee.Rsvp));
                    cmdInsertAttendance.Parameters.Add(new SqlParameter("@occurrenceid", occurrenceid));
                    cmdInsertAttendance.ExecuteNonQuery();
                }

                // Add to Guest
                SqlCommand cmdInsertGuest = new SqlCommand($@"
                INSERT INTO attendanceguests([createddatetime],[count],[createdbypersonaliasid],[sundaydatetime],[occurrenceid])
                    VALUES(@createddatetime, @count, @createdbypersonaliasid, @sundaydatetime,@occurrenceid)", _connection);
                cmdInsertGuest.Parameters.Add(new SqlParameter("@createddatetime", attendance[0].CreatedDateTime));
                cmdInsertGuest.Parameters.Add(new SqlParameter("@count", guestCount));
                cmdInsertGuest.Parameters.Add(new SqlParameter("@createdbypersonaliasid", attendance[0].CreatedByPersonAliasId));
                cmdInsertGuest.Parameters.Add(new SqlParameter("@sundaydatetime", sundaydate));
                cmdInsertGuest.Parameters.Add(new SqlParameter("@occurrenceid", occurrenceid));
                cmdInsertGuest.ExecuteNonQuery();
                
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
