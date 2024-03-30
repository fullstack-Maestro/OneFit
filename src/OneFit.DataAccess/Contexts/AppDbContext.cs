﻿using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace OneFit.DataAccess.Contexts;

public class AppDbContext(IConfiguration configuration)
{
    public IDbConnection CreateConnection()
        => new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));

    public async Task Init()
    {
        await InitDatabase();
        await InitTables();
    }

    private async Task InitDatabase()
    {
        using var connection = CreateConnection();
        var sqlDbCount = $"SELECT COUNT(*) FROM pg_database WHERE datname = 'OneFitDb'";
        var dbCount = await connection.ExecuteScalarAsync<int>(sqlDbCount);

        if (dbCount == 0)
        {
            var sql = $"CREATE DATABASE OneFitDb";
            await connection.ExecuteAsync(sql);
        }
    }

    private async Task InitTables()
    {
        using var connection = CreateConnection();
        await _initUsers();
        await _initCategories();
        await _initStudios();
        await _initFacilities();
        await _initStudioFacilities();
        await _initEnrollments();

        async Task _initUsers()
        {
            var sql = """
                CREATE TABLE IF NOT EXISTS "Users" (
                "Id" bigint PRIMARY KEY,
                "FirstName" varchar,
                "LastrName" varchar,
                "Phone" varchar,
                "Password" varchar
            );
            """;

            await connection.ExecuteAsync(sql);
        }

        async Task _initCategories()
        {
            var sql = """
                CREATE TABLE IF NOT EXISTS "Categories" (
                "Id" bigint PRIMARY KEY,
                "Name" varchar,
                "CreatedAt" timestamp default current_timestamp,
                "UpdatedAt" timestamp default null
            );
            """;

            await connection.ExecuteAsync(sql);
        }

        async Task _initStudios()
        {
            var sql = """
                CREATE TABLE IF NOT EXISTS "Studios" (
                "Id" bigint PRIMARY KEY,
                "Name" varchar,
                "Description" varchar,
                "Address" varchar,
                "Type" int,
                "CategoryId" bigint,
                "CreatedAt" timestamp default current_timestamp,
                "UpdatedAt" timestamp default null
            );
            """;

            await connection.ExecuteAsync(sql);
        }

        async Task _initFacilities()
        {
            var sql = """
                CREATE TABLE IF NOT EXISTS "Facilities" (
                "Id" bigint PRIMARY KEY,
                "Name" varchar,
                "CreatedAt" timestamp default current_timestamp,
                "UpdatedAt" timestamp default null
            );
            """;

            await connection.ExecuteAsync(sql);
        }

        async Task _initStudioFacilities()
        {
            var sql = """
                CREATE TABLE IF NOT EXISTS "StudioFacilities" (
                "Id" bigint PRIMARY KEY,
                "StudioId" bigint,
                "FacilityId" bigint,
                "CreatedAt" timestamp default current_timestamp,
                "UpdatedAt" timestamp default null
            );
            """;

            await connection.ExecuteAsync(sql);
        }

        async Task _initEnrollments()
        {
            var sql = """
                CREATE TABLE IF NOT EXISTS "Enrollments" (
                "Id" bigint PRIMARY KEY,
                "StudioId" bigint,
                "UserId" bigint,
                "CreatedAt" timestamp default current_timestamp,
                "UpdatedAt" timestamp default null
            );
            """;

            await connection.ExecuteAsync(sql);
        }
    }
}