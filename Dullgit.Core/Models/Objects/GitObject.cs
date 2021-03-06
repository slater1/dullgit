﻿namespace Dullgit.Core.Models.Objects
{
  /// <summary>
  /// Models a Git object
  /// https://git-scm.com/book/en/v2/Git-Internals-Git-Objects
  /// </summary>
  public class GitObject
  {
    public virtual string Value { get; protected set; }

    public GitObject() { }

  }
}
