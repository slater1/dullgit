﻿using Dullgit.Core;
using Dullgit.Core.Models.Objects;
using Dullgit.Data.Filters;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dullgit.Data
{
  public class FileRepo : IRepo
  {
    private readonly IObjectFactory _objectFactory;
    private readonly IContentFilter[] _filters;

    public string Dir => ".dg";
    public string FullPath => Path.Combine(Directory.GetCurrentDirectory(), Dir);
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public FileRepo(IObjectFactory objectFactory, params IContentFilter[] filters)
    {
      _objectFactory = objectFactory;
      _filters = filters;
    }

    public bool Exists() => Directory.Exists(FullPath);

    public async Task<string> HashAsync(string path, ObjectType type)
    {
      string content = await FileExtensions.ReadFileAsync(path, Encoding).ConfigureAwait(false);
      string filtered = Filter(content);
      string blob = _objectFactory.Create(type, filtered).Value;

      string oid = Hash(Encoding.GetBytes(blob));
      string[] split = oid.Split(2);

      await FileExtensions.WriteFileAsync($"{Dir}/objects/{split[0]}/{split[1]}", content);

      return oid;
    }

    public async Task<string> GetObjectAsync(string oid)
    {
      string[] split = oid.Split(2);
      return await FileExtensions.ReadFileAsync($"{Dir}/objects/{split[0]}/{split[1]}", Encoding);
    }

    private string Filter(string data)
    {
      foreach (IContentFilter filter in _filters)
      {
        data = filter.Run(data);
      }

      return data;
    }

    private string Hash(byte[] data) => data.Hash().AsString();

    public async Task<bool> InitAsync(CancellationToken ct = default) 
      => await Task
        .Run(() => DirectoryExtensions.CreateSafely(FullPath), ct)
        .ConfigureAwait(false);
  }
}
