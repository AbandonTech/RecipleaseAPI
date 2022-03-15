using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Reciplease;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public string DbPath { get; }

    public UserContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "reciplease.db");     
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class User{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
