#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:18 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace InlineFormParser.Zip
{
	public class FileSelector
	{
		internal SelectionCriterion _Criterion;

		public FileSelector(string selectionCriteria)
			: this(selectionCriteria, true)
		{
		}

		public FileSelector(string selectionCriteria, bool traverseDirectoryReparsePoints)
		{
			if (!string.IsNullOrEmpty(selectionCriteria)) _Criterion = _ParseCriterion(selectionCriteria);
			TraverseReparsePoints = traverseDirectoryReparsePoints;
		}

		public string SelectionCriteria
		{
			get
			{
				if (_Criterion == null) return null;
				return _Criterion.ToString();
			}
			set
			{
				if (value == null)
					_Criterion = null;
				else if (value.Trim() == string.Empty)
					_Criterion = null;
				else
					_Criterion = _ParseCriterion(value);
			}
		}

		public bool TraverseReparsePoints { get; set; }

		private static string NormalizeCriteriaExpression(string source)
		{
			string[][] array = new string[11][]
			{
				new string[2]
				{
					"([^']*)\\(\\(([^']+)",
					"$1( ($2"
				},
				new string[2]
				{
					"(.)\\)\\)",
					"$1) )"
				},
				new string[2]
				{
					"\\((\\S)",
					"( $1"
				},
				new string[2]
				{
					"(\\S)\\)",
					"$1 )"
				},
				new string[2]
				{
					"^\\)",
					" )"
				},
				new string[2]
				{
					"(\\S)\\(",
					"$1 ("
				},
				new string[2]
				{
					"\\)(\\S)",
					") $1"
				},
				new string[2]
				{
					"(=)('[^']*')",
					"$1 $2"
				},
				new string[2]
				{
					"([^ !><])(>|<|!=|=)",
					"$1 $2"
				},
				new string[2]
				{
					"(>|<|!=|=)([^ =])",
					"$1 $2"
				},
				new string[2]
				{
					"/",
					"\\"
				}
			};
			string input = source;
			for (int i = 0; i < array.Length; i++)
			{
				string pattern = "(?<=(?:[^']*'[^']*')*[^']*)" + array[i][0] + "(?=(?:[^']*'[^']*')*[^']*$)";
				input = Regex.Replace(input, pattern, array[i][1]);
			}

			string pattern2 = "/(?=[^']*'(?:[^']*'[^']*')*[^']*$)";
			input = Regex.Replace(input, pattern2, "\\");
			pattern2 = " (?=[^']*'(?:[^']*'[^']*')*[^']*$)";
			return Regex.Replace(input, pattern2, "\u0006");
		}

		private static SelectionCriterion _ParseCriterion(string s)
		{
			if (s == null) return null;
			s = NormalizeCriteriaExpression(s);
			if (s.IndexOf(" ", StringComparison.Ordinal) == -1) s = $"name = {s}";
			string[] array = s.Trim().Split(' ', '\t');
			if (array.Length < 3) throw new ArgumentException(s);
			SelectionCriterion selectionCriterion = null;
			Stack<ParseState> stack = new Stack<ParseState>();
			Stack<SelectionCriterion> stack2 = new Stack<SelectionCriterion>();
			stack.Push(ParseState.Start);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].ToLower();
				ParseState parseState;
				switch (text)
				{
					case "and":
					case "xor":
					case "or":
					{
						parseState = stack.Peek();
						if (parseState != ParseState.CriterionDone)
							throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						if (array.Length <= i + 3) throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						LogicalConjunction conjunction =
							(LogicalConjunction) Enum.Parse(typeof(LogicalConjunction), array[i].ToUpper(), true);
						CompoundCriterion compoundCriterion = new CompoundCriterion();
						compoundCriterion.Left = selectionCriterion;
						compoundCriterion.Right = null;
						compoundCriterion.Conjunction = conjunction;
						selectionCriterion = compoundCriterion;
						stack.Push(parseState);
						stack.Push(ParseState.ConjunctionPending);
						stack2.Push(selectionCriterion);
						break;
					}
					case "(":
						parseState = stack.Peek();
						if (parseState != 0 && parseState != ParseState.ConjunctionPending && parseState != ParseState.OpenParen)
							throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						if (array.Length <= i + 4) throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						stack.Push(ParseState.OpenParen);
						break;
					case ")":
						parseState = stack.Pop();
						if (stack.Peek() != ParseState.OpenParen)
							throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						stack.Pop();
						stack.Push(ParseState.CriterionDone);
						break;
					case "atime":
					case "ctime":
					case "mtime":
					{
						if (array.Length <= i + 2) throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						DateTime value;
						try
						{
							value = DateTime.ParseExact(array[i + 2], "yyyy-MM-dd-HH:mm:ss", null);
						}
						catch (FormatException)
						{
							try
							{
								value = DateTime.ParseExact(array[i + 2], "yyyy/MM/dd-HH:mm:ss", null);
							}
							catch (FormatException)
							{
								try
								{
									value = DateTime.ParseExact(array[i + 2], "yyyy/MM/dd", null);
								}
								catch (FormatException)
								{
									try
									{
										value = DateTime.ParseExact(array[i + 2], "MM/dd/yyyy", null);
									}
									catch (FormatException)
									{
										value = DateTime.ParseExact(array[i + 2], "yyyy-MM-dd", null);
									}
								}
							}
						}

						value = DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime();
						TimeCriterion timeCriterion = new TimeCriterion();
						timeCriterion.Which = (WhichTime) Enum.Parse(typeof(WhichTime), array[i], true);
						timeCriterion.Operator = (ComparisonOperator) EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
						timeCriterion.Time = value;
						selectionCriterion = timeCriterion;
						i += 2;
						stack.Push(ParseState.CriterionDone);
						break;
					}
					case "length":
					case "size":
					{
						if (array.Length <= i + 2) throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						string text2 = array[i + 2];
						long size = !text2.ToUpper().EndsWith("K")
							? (!text2.ToUpper().EndsWith("KB")
								? (!text2.ToUpper().EndsWith("M")
									? (!text2.ToUpper().EndsWith("MB")
										? (!text2.ToUpper().EndsWith("G")
											? (!text2.ToUpper().EndsWith("GB")
												? long.Parse(array[i + 2])
												: long.Parse(text2.Substring(0, text2.Length - 2)) * 1024 * 1024 * 1024)
											: long.Parse(text2.Substring(0, text2.Length - 1)) * 1024 * 1024 * 1024)
										: long.Parse(text2.Substring(0, text2.Length - 2)) * 1024 * 1024)
									: long.Parse(text2.Substring(0, text2.Length - 1)) * 1024 * 1024)
								: long.Parse(text2.Substring(0, text2.Length - 2)) * 1024)
							: long.Parse(text2.Substring(0, text2.Length - 1)) * 1024;
						SizeCriterion sizeCriterion = new SizeCriterion();
						sizeCriterion.Size = size;
						sizeCriterion.Operator = (ComparisonOperator) EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
						selectionCriterion = sizeCriterion;
						i += 2;
						stack.Push(ParseState.CriterionDone);
						break;
					}
					case "filename":
					case "name":
					{
						if (array.Length <= i + 2) throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						ComparisonOperator comparisonOperator2 =
							(ComparisonOperator) EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
						if (comparisonOperator2 != ComparisonOperator.NotEqualTo && comparisonOperator2 != ComparisonOperator.EqualTo)
							throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						string text3 = array[i + 2];
						if (text3.StartsWith("'") && text3.EndsWith("'"))
							text3 = text3.Substring(1, text3.Length - 2).Replace("\u0006", " ");
						NameCriterion nameCriterion = new NameCriterion();
						nameCriterion.MatchingFileSpec = text3;
						nameCriterion.Operator = comparisonOperator2;
						selectionCriterion = nameCriterion;
						i += 2;
						stack.Push(ParseState.CriterionDone);
						break;
					}
					case "attrs":
					case "attributes":
					case "type":
					{
						if (array.Length <= i + 2) throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						ComparisonOperator comparisonOperator =
							(ComparisonOperator) EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
						if (comparisonOperator != ComparisonOperator.NotEqualTo && comparisonOperator != ComparisonOperator.EqualTo)
							throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
						object obj;
						if (!(text == "type"))
						{
							AttributesCriterion attributesCriterion = new AttributesCriterion();
							attributesCriterion.AttributeString = array[i + 2];
							attributesCriterion.Operator = comparisonOperator;
							obj = attributesCriterion;
						}
						else
						{
							TypeCriterion typeCriterion = new TypeCriterion();
							typeCriterion.AttributeString = array[i + 2];
							typeCriterion.Operator = comparisonOperator;
							obj = typeCriterion;
						}

						selectionCriterion = (SelectionCriterion) obj;
						i += 2;
						stack.Push(ParseState.CriterionDone);
						break;
					}
					case "":
						stack.Push(ParseState.Whitespace);
						break;
					default:
						throw new ArgumentException("'" + array[i] + "'");
				}

				parseState = stack.Peek();
				if (parseState == ParseState.CriterionDone)
				{
					stack.Pop();
					if (stack.Peek() == ParseState.ConjunctionPending)
						while (stack.Peek() == ParseState.ConjunctionPending)
						{
							CompoundCriterion compoundCriterion2 = stack2.Pop() as CompoundCriterion;
							compoundCriterion2.Right = selectionCriterion;
							selectionCriterion = compoundCriterion2;
							stack.Pop();
							parseState = stack.Pop();
							if (parseState != ParseState.CriterionDone) throw new ArgumentException("??");
						}
					else
						stack.Push(ParseState.CriterionDone);
				}

				if (parseState == ParseState.Whitespace) stack.Pop();
			}

			return selectionCriterion;
		}

		public override string ToString()
		{
			return $"FileSelector({_Criterion})";
		}

		public bool Evaluate(string filename)
		{
			return _Criterion.Evaluate(filename);
		}

		[Conditional("SelectorTrace")]
		private void SelectorTrace(string format, params object[] args)
		{
			if (_Criterion != null && _Criterion.Verbose) Console.WriteLine(format, args);
		}

		public ICollection<string> SelectFiles(string directory)
		{
			return SelectFiles(directory, false);
		}

		public ReadOnlyCollection<string> SelectFiles(string directory, bool recurseDirectories)
		{
			if (_Criterion == null) throw new ArgumentException("SelectionCriteria has not been set");
			List<string> list = new List<string>();
			try
			{
				if (Directory.Exists(directory))
				{
					string[] files = Directory.GetFiles(directory);
					string[] array = files;
					foreach (string text in array)
						if (Evaluate(text))
							list.Add(text);
					if (recurseDirectories)
					{
						string[] directories = Directory.GetDirectories(directory);
						string[] array2 = directories;
						foreach (string text2 in array2)
							if (TraverseReparsePoints || (File.GetAttributes(text2) & FileAttributes.ReparsePoint) == 0)
							{
								if (Evaluate(text2)) list.Add(text2);
								list.AddRange(SelectFiles(text2, true));
							}
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (IOException)
			{
			}

			return list.AsReadOnly();
		}

		public ICollection<ZipEntry> SelectEntries(ZipFile zip)
		{
			if (zip == null) throw new ArgumentNullException(nameof(zip));
			List<ZipEntry> list = new List<ZipEntry>();
			foreach (ZipEntry item in zip)
				if (Evaluate(item))
					list.Add(item);
			return list;
		}

		public ICollection<ZipEntry> SelectEntries(ZipFile zip, string directoryPathInArchive)
		{
			if (zip == null) throw new ArgumentNullException(nameof(zip));
			List<ZipEntry> list = new List<ZipEntry>();
			string text = directoryPathInArchive == null ? null : directoryPathInArchive.Replace("/", "\\");
			if (!string.IsNullOrEmpty(text))
				while (text.EndsWith("\\"))
					text = text.Substring(0, text.Length - 1);
			foreach (ZipEntry item in zip)
				if ((directoryPathInArchive == null ||
				     string.Equals(Path.GetDirectoryName(item.FileName), directoryPathInArchive,
					     StringComparison.InvariantCultureIgnoreCase) || string.Equals(Path.GetDirectoryName(item.FileName), text,
					     StringComparison.InvariantCultureIgnoreCase)) && Evaluate(item))
					list.Add(item);
			return list;
		}

		private bool Evaluate(ZipEntry entry)
		{
			if (_Criterion == null) return true;
			return _Criterion.Evaluate(entry);
		}

		private enum ParseState
		{
			Start,
			OpenParen,
			CriterionDone,
			ConjunctionPending,
			Whitespace
		}

		private static class RegexAssertions
		{
			public const string FollowedByOddNumberOfSingleQuotesAndLineEnd = "(?=[^']*'(?:[^']*'[^']*')*[^']*$)";

			public const string PrecededByEvenNumberOfSingleQuotes = "(?<=(?:[^']*'[^']*')*[^']*)";

			public const string FollowedByEvenNumberOfSingleQuotesAndLineEnd = "(?=(?:[^']*'[^']*')*[^']*$)";
		}
	}
}