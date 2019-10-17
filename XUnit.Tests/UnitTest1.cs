using FluentAssertions;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace CSharp_SQLitePCLRaw.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var builder = InMemory.CreateConnection();
            builder.Open();
            var comm = builder.CreateCommand();
            Utils.CreateTable(comm);
            comm.Dispose();
            comm = builder.CreateCommand();
            Utils.InsertData(comm);
            comm.Dispose();
            comm = builder.CreateCommand();
            var a = Utils.ReadData(comm);
            a.Should().Equals("Test Text");

            builder.Dispose();
        }
    }

    public static class Utils
    {
        public static void CreateTable(SqliteCommand command)
        {
            string Createsql = "CREATE TABLE SampleTable (Col1 VARCHAR(20), Col2 INT)";
            string Createsql1 = "CREATE TABLE SampleTable1 (Col1 VARCHAR(20), Col2 INT)";
            command.CommandText = Createsql;
            command.ExecuteNonQuery();
            command.CommandText = Createsql1;
            command.ExecuteNonQuery();
        }

        public static void InsertData(SqliteCommand command)
        {
            command.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test Text ', 1); ";
            command.ExecuteNonQuery();
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test2 Text2 ', 3); ";
            command.ExecuteNonQuery();


            command.CommandText = "INSERT INTO SampleTable1 (Col1, Col2) VALUES('Test3 Text3 ', 3); ";
            command.ExecuteNonQuery();

        }

        public static string ReadData(SqliteCommand command)
        {
            command.CommandText = "SELECT * FROM SampleTable";

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                return reader.GetString(0);
            }

            return "";
        }
    }

    public static class InMemory
    {
        public const string DataSource = ":memory:";

        public static SqliteConnection CreateConnection()
        {
            var builder = new SqliteConnectionStringBuilder();

            builder.SetInMemory();

            return new SqliteConnection(builder.ConnectionString);
        }

        public static void Execute(Action<SqliteConnection> action)
        {
            using (var connection = CreateConnection())
            {
                var opened = connection.State == ConnectionState.Open;

                if (!opened) connection.Open();

                try
                {
                    action(connection);
                }
                finally
                {
                    if (!opened) connection.Close();
                }
            }
        }
    }

    public static class SqliteConnectionStringBuilderExtensions
    {
        public static void SetInMemory(this SqliteConnectionStringBuilder builder)
        {
            builder.DataSource = InMemory.DataSource;
        }
    }
}
