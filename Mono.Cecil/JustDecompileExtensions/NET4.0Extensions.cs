using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

//TELERIK AUTHORSHIP
#if !NET_4_0
/// <summary>
/// Providing NEt 4.0 and above functionality as extension methods. Used in 3.5 builds.
/// </summary>
    public static class NET4Extensions
    {
        /// <summary>
        /// Duplicate of .NET 4.0 String.IsNullOrWhitespace. Avoiding problems with the .net 3.5 build.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value != null)
            {
                int num = 0;
                while (num < value.Length)
                {
                    if (char.IsWhiteSpace(value[num]))
                    {
                        num++;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        public static string Combine(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == null)
                {
                    throw new ArgumentNullException("paths");
                }
                if (paths[i].Length != 0)
                {

                    for (int l = 0; l < paths[i].Length; l++)
                    {
                        int n = (int)paths[i][l];
                        if (n == 34 || n == 60 || n == 62 || n == 124 || n < 32)
                        {
                            throw new ArgumentException("Argument_InvalidPathChars");
                        }
                    }

                    if (Path.IsPathRooted(paths[i]))
                    {
                        num2 = i;
                        num = paths[i].Length;
                    }
                    else
                    {
                        num += paths[i].Length;
                    }
                    char c = paths[i][paths[i].Length - 1];
                    if (c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar && c != Path.VolumeSeparatorChar)
                    {
                        num++;
                    }
                }
            }
            StringBuilder stringBuilder = new StringBuilder(num);
            for (int j = num2; j < paths.Length; j++)
            {
                if (paths[j].Length != 0)
                {
                    if (stringBuilder.Length == 0)
                    {
                        stringBuilder.Append(paths[j]);
                    }
                    else
                    {
                        char c2 = stringBuilder[stringBuilder.Length - 1];
                        if (c2 != Path.DirectorySeparatorChar && c2 != Path.AltDirectorySeparatorChar && c2 != Path.VolumeSeparatorChar)
                        {
                            stringBuilder.Append(Path.DirectorySeparatorChar);
                        }
                        stringBuilder.Append(paths[j]);
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }


        public static class StreamExtensions
        {
              public static void CopyTo(this Stream source, Stream destination)
              {
                  if (destination != null)
                  {
                      if (source.CanRead || source.CanWrite)
                      {
                          if (destination.CanRead || destination.CanWrite)
                          {
                              if (source.CanRead)
                              {
                                  if (destination.CanWrite)
                                  {
                                      byte[] numArray = new byte[4096];
                                      while (true)
                                      {
                                          int num = source.Read(numArray, 0, (int)numArray.Length);
                                          int num1 = num;
                                          if (num == 0)
                                          {
                                              break;
                                          }
                                          destination.Write(numArray, 0, num1);
                                      }
                                      return;
                                  }
                                  else
                                  {
                                      throw new NotSupportedException("NotSupported_UnwritableStream");
                                  }
                              }
                              else
                              {
                                  throw new NotSupportedException("NotSupported_UnreadableStream");
                              }
                          }
                          else
                          {
                              throw new ObjectDisposedException("destination", "ObjectDisposed_StreamClosed");
                          }
                      }
                      else
                      {
                          throw new ObjectDisposedException(null, "ObjectDisposed_StreamClosed");
                      }
                  }
                  else
                  {
                      throw new ArgumentNullException("destination");
                  }
              }
        }

        

#endif
