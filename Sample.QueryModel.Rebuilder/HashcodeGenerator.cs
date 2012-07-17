using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using System.Security.Cryptography;

namespace Sample.QueryModel.Rebuilder
{
    public interface IHashcodeGenerator
    {
        string Generate(Type t);
    }

    /// <summary>
    /// a class used to compute a type hashcode, we will consider class members and their instructions
    /// </summary>
    public class HashcodeGenerator : IHashcodeGenerator
    {
        public string Generate(Type t)
        {
            var location = t.Assembly.Location;
            var mod = ModuleDefinition.ReadModule(location);
            var builder = new StringBuilder();

            var typeDefinition = mod.GetType(t.FullName);
            builder.AppendLine(typeDefinition.Name);
            ProcessMembers(builder, typeDefinition);

            // we include nested types
            foreach (var nested in typeDefinition.NestedTypes)
            {
                ProcessMembers(builder, nested);
            }

            return CalculateMD5Hash(builder.ToString());
        }

        static void ProcessMembers(StringBuilder builder, TypeDefinition typeDefinition)
        {
            foreach (var field in typeDefinition.Fields.OrderBy(f => f.Name))
            {
                builder.AppendLine("  " + field);
            }
            foreach (var property in typeDefinition.Properties.OrderBy(p => p.Name))
            {
                builder.AppendLine(" " + property);
            }
            foreach (var md in typeDefinition.Methods.OrderBy(m => m.Name))
            {
                builder.AppendLine(" " + md);

                foreach (var instruction in md.Body.Instructions)
                {
                    // we don't care about offsets
                    instruction.Offset = 0;
                    builder.AppendLine(" " + instruction);
                }
            }
        }

		/// <summary>
		/// is this good enough to have low collision ?
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		static string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}
    }
}
