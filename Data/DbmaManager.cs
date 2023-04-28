using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BloodBank_Project.Data
{
	public class DbmaManager
	{
		string connstring;
		SqlConnection sqlConnection;
		public DbmaManager()
		{
			connstring = @"Data Source=HARRY-PC\SQLEXPRESS;Initial Catalog=BloodBank;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
			sqlConnection = new SqlConnection(connstring);
			sqlConnection.Open();
		}

		public async Task<(int, int, int, int, int, int, int, int)> GetBloodRecordsAsync()
		{
			int oNeg, oPos, aNeg, aPos, bNeg, bPos, abNeg, abPos;
			oNeg = oPos = aNeg = aPos = bNeg = bPos = abNeg = abPos = 0;

			using (SqlCommand command = new SqlCommand("SELECT * FROM total_blood_record", sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						oNeg = (int)reader["oneg"];
						oPos = (int)reader["opos"];
						aNeg = (int)reader["an"];
						aPos = (int)reader["ap"];
						bNeg = (int)reader["bn"];
						bPos = (int)reader["bp"];
						abNeg = (int)reader["abn"];
						abPos = (int)reader["abp"];
					}
				}
			}

			return (oNeg, oPos, aNeg, aPos, bNeg, bPos, abNeg, abPos);
		}
		public async Task<int> GetIssuesCountForCurrentMonthAsync()
		{
			int count = 0;
			string query = @"
        SELECT COUNT(*)
        FROM issued
        WHERE MONTH(date) = MONTH(GETDATE()) AND YEAR(date) = YEAR(GETDATE())";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					if (reader.Read())
					{
						count = reader.GetInt32(0);
					}
				}
			}

			return count;
		}
		public async Task<int> GetDonationsCountForCurrentMonthAsync()
		{
			int count = 0;
			string query = @"
        SELECT COUNT(*)
        FROM donations
        WHERE MONTH(date) = MONTH(GETDATE()) AND YEAR(date) = YEAR(GETDATE())";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					if (reader.Read())
					{
						count = reader.GetInt32(0);
					}
				}
			}

			return count;
		}
		public async Task<bool> InsertIssuedRequestAsync(IIssuedRequest request)
		{
			string query = @"
        INSERT INTO issued (name, mobile, hospital, bgroup, units, reason, date)
        VALUES (@Name, @Mobile, @Hospital, @BloodGroup, @Units, @Purpose, @DateOfIssue)";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				command.Parameters.AddWithValue("@Name", request.Name);
				command.Parameters.AddWithValue("@Mobile", request.Mobile);
				command.Parameters.AddWithValue("@Hospital", request.Hospital);
				command.Parameters.AddWithValue("@BloodGroup", request.BloodGroup);
				command.Parameters.AddWithValue("@Units", request.Units);
				command.Parameters.AddWithValue("@Purpose", request.Purpose);
				command.Parameters.AddWithValue("@DateOfIssue", request.DateOfIssue);

				int result = await command.ExecuteNonQueryAsync();
				return result > 0;
			}
		}
		public async Task<bool> IsBloodAvailableAsync(string bloodGroup, int units)
		{
			string columnName = GetColumnNameForBloodGroup(bloodGroup);

			string query = $"SELECT {columnName} FROM total_blood_record";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						int availableUnits = reader.GetInt32(0);
						return availableUnits >= units;
					}
					else
					{
						return false;
					}
				}
			}
		}
		private string GetColumnNameForBloodGroup(string bloodGroup)
		{
			switch (bloodGroup)
			{
				case "O-":
					return "oneg";
				case "O+":
					return "opos";
				case "A-":
					return "an";
				case "A+":
					return "ap";
				case "B-":
					return "bn";
				case "B+":
					return "bp";
				case "AB-":
					return "abn";
				case "AB+":
					return "abp";
				default:
					throw new ArgumentException("Invalid blood group");
			}
		}
		public async Task<(bool, int)> UpdateBloodUnitsAsync(string bloodGroup, int units)
		{
			string columnName = GetColumnNameForBloodGroup(bloodGroup);

			string checkQuery = $"SELECT {columnName} FROM total_blood_record";
			int availableUnits = 0;

			using (SqlCommand command = new SqlCommand(checkQuery, sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						availableUnits = reader.GetInt32(0);
					}
				}
			}

			if (availableUnits < units)
			{
				return (false, availableUnits);
			}

			string updateQuery = $"UPDATE total_blood_record SET {columnName} = {columnName} - @Units";

			using (SqlCommand command = new SqlCommand(updateQuery, sqlConnection))
			{
				command.Parameters.AddWithValue("@Units", units);

				int result = await command.ExecuteNonQueryAsync();
				return (result > 0, availableUnits - units);
			}
		}

		public async Task<bool> RegisterDonorAsync(Idonor donor)
		{
			string query = @"
            INSERT INTO donormaster (mobile, name, gender, address, city, bgroup, age)
            VALUES (@Mobile, @Name, @Gender, @Address, @City, @BloodGroup, @Age)";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				command.Parameters.AddWithValue("@Mobile", donor.PhoneNumber);
				command.Parameters.AddWithValue("@Name", donor.Name);
				command.Parameters.AddWithValue("@Gender", donor.Gender);
				command.Parameters.AddWithValue("@Address", donor.Address);
				command.Parameters.AddWithValue("@City", donor.City);
				command.Parameters.AddWithValue("@BloodGroup", donor.BloodGroup);
				command.Parameters.AddWithValue("@Age", donor.Age);

				int result = await command.ExecuteNonQueryAsync();
				return result > 0;
			}
		}

		public async Task<Donor> FetchDonorAsync(string phoneNumber)
		{
			Donor donor = null;
			string query = @"
        SELECT *
        FROM donormaster
        WHERE mobile = @Mobile";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				command.Parameters.AddWithValue("@Mobile", phoneNumber);

				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						donor = new Donor
						{
							PhoneNumber = reader["mobile"].ToString(),
							Name = reader["name"].ToString(),
							Gender = reader["gender"].ToString(),
							Address = reader["address"].ToString(),
							City = reader["city"].ToString(),
							BloodGroup = reader["bgroup"].ToString(),
							Age = int.Parse(reader["age"].ToString())
						};
					}
				}
			}

			return donor;
		}

		public async Task<bool> UpdateDonorAsync(Donor donor)
		{
			string query = @"
        UPDATE donormaster
        SET name = @Name, gender = @Gender, address = @Address, city = @City, bgroup = @BloodGroup, age = @Age
        WHERE mobile = @Mobile";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				command.Parameters.AddWithValue("@Mobile", donor.PhoneNumber);
				command.Parameters.AddWithValue("@Name", donor.Name);
				command.Parameters.AddWithValue("@Gender", donor.Gender);
				command.Parameters.AddWithValue("@Address", donor.Address);
				command.Parameters.AddWithValue("@City", donor.City);
				command.Parameters.AddWithValue("@BloodGroup", donor.BloodGroup);
				command.Parameters.AddWithValue("@Age", donor.Age);

				int result = await command.ExecuteNonQueryAsync();
				return result > 0;
			}
		}

		public async Task<bool> DeleteDonorAsync(string phoneNumber)
		{
			string query = @"
        DELETE FROM donormaster
        WHERE mobile = @Mobile";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				command.Parameters.AddWithValue("@Mobile", phoneNumber);

				int result = await command.ExecuteNonQueryAsync();
				return result > 0;
			}
		}
		public async Task<Donor> FetchDonorsAsync(string phoneNumber)
		{
			Donor donor = null;

			string query = @"
        SELECT * FROM donormaster
        WHERE mobile = @PhoneNumber";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						donor = new Donor()
						{
							PhoneNumber = reader["mobile"].ToString(),
							Name = reader["name"].ToString(),
							Gender = reader["gender"].ToString(),
							Address = reader["address"].ToString(),
							City = reader["city"].ToString(),
							BloodGroup = reader["bgroup"].ToString(),
							Age = (int)reader["age"]
						};
					}
				}
			}

			return donor;
		}

		public async Task<bool> SaveDonationAsync(Idonate donation)
		{
			string query = @"
        INSERT INTO Donations (mobile, bgroup, date, noofunits)
        VALUES (@Mobile, @BloodGroup, @DateOfDonation, @NoOfUnits)";

			using (SqlCommand command = new SqlCommand(query, sqlConnection))
			{
				command.Parameters.AddWithValue("@Mobile", donation.PhoneNumber);
				command.Parameters.AddWithValue("@BloodGroup", donation.BloodGroup);
				command.Parameters.AddWithValue("@DateOfDonation", donation.DateOfDonation);
				command.Parameters.AddWithValue("@NoOfUnits", donation.NoOfUnits);

				int result = await command.ExecuteNonQueryAsync();
				return result > 0;
			}
		}
		public async Task<bool> UpdateBloodUnitsForDonationAsync(string bloodGroup, int units)
		{
			string columnName = GetColumnNameForBloodGroup(bloodGroup);

			string updateQuery = $"UPDATE total_blood_record SET {columnName} = {columnName} + @Units";

			using (SqlCommand command = new SqlCommand(updateQuery, sqlConnection))
			{
				command.Parameters.AddWithValue("@Units", units);

				int result = await command.ExecuteNonQueryAsync();
				return result > 0;
			}
		}
		public async Task<List<Donor>> FetchAllDonorsAsync()
		{
			List<Donor> donors = new List<Donor>();
			using (SqlCommand command = new SqlCommand("SELECT * FROM donormaster", sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						donors.Add(new Donor
						{
							PhoneNumber = reader["mobile"].ToString(),
							Name = reader["name"].ToString(),
							Gender = reader["gender"].ToString(),
							Address = reader["address"].ToString(),
							City = reader["city"].ToString(),
							BloodGroup = reader["bgroup"].ToString(),
							Age = int.TryParse(reader["age"].ToString(), out int age) ? age : 0
						});
					}
				}
			}
			return donors;
		}

		public async Task<List<IssuedRequest>> FetchAllIssuesAsync()
		{
			List<IssuedRequest> issues = new List<IssuedRequest>();
			using (SqlCommand command = new SqlCommand("SELECT * FROM issued", sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						issues.Add(new IssuedRequest
						{
							Name = reader["name"].ToString(),
							Mobile = reader["mobile"].ToString(),
							Hospital = reader["hospital"].ToString(),
							BloodGroup = reader["bgroup"].ToString(),
							Units = int.TryParse(reader["units"].ToString(), out int units) ? units : 0,
							Purpose = reader["reason"].ToString(),
							DateOfIssue = (DateTime)reader["date"]
						});
					}
				}
			}
			return issues;
		}

		public async Task<List<Donate>> FetchAllDonationsAsync()
		{
			List<Donate> donations = new List<Donate>();
			using (SqlCommand command = new SqlCommand("SELECT * FROM donations", sqlConnection))
			{
				using (SqlDataReader reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						donations.Add(new Donate
						{
							PhoneNumber = reader["mobile"].ToString(),
							BloodGroup = reader["bgroup"].ToString(),
							DateOfDonation = (DateTime)reader["date"],
							NoOfUnits = int.TryParse(reader["noofunits"].ToString(), out int noOfUnits) ? noOfUnits : 0
						});
					}
				}
			}
			return donations;
		}


	}

}
