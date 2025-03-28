
using System;
using Cysharp.Text;

namespace Radish.Logging
{
	internal sealed class RadishLogger : ILogger
	{
		public string Name { get; }

		internal RadishLogger(string name)
		{
			Name = name;
		}

		#region Static Write Methods

		private static void Write(LogLevel level, string category, string message)
		{
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, message.AsSpan());
			}
		}

		private static void WriteException(LogLevel level, string category, Exception ex)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException(LogLevel level, string category, Exception ex, string message)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendLine(message);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1>(LogLevel level, string category, string format, T1 arg1)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1>(LogLevel level, string category, Exception ex, string format, T1 arg1)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2>(LogLevel level, string category, string format, T1 arg1, T2 arg2)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			private static void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogLevel level, string category, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

		private static void WriteException<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogLevel level, string category, Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			using var fmt = ZString.CreateStringBuilder();
			fmt.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
			fmt.Append(' ');
			fmt.Append(ex);
			var span = fmt.AsSpan();
			foreach (var writer in LogManager.RegisteredLoggers)
			{
				writer.Write(level, category, span);
			}
		}

			#endregion

		#region Info (Interface)

		public void Info(string message)
			=> Write(LogLevel.Info, Name, message);

			public void Info<T1>(string format, T1 arg1) 
			=> Write(LogLevel.Info, Name, format, arg1);

			public void Info<T1, T2>(string format, T1 arg1, T2 arg2) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2);

			public void Info<T1, T2, T3>(string format, T1 arg1, T2 arg2, T3 arg3) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3);

			public void Info<T1, T2, T3, T4>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4);

			public void Info<T1, T2, T3, T4, T5>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5);

			public void Info<T1, T2, T3, T4, T5, T6>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6);

			public void Info<T1, T2, T3, T4, T5, T6, T7>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);

			public void Info<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) 
			=> Write(LogLevel.Info, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);

			#endregion

		#region Warning (Interface)

		public void Warn(string message)
			=> Write(LogLevel.Warning, Name, message);

			public void Warn<T1>(string format, T1 arg1) 
			=> Write(LogLevel.Warning, Name, format, arg1);

			public void Warn<T1, T2>(string format, T1 arg1, T2 arg2) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2);

			public void Warn<T1, T2, T3>(string format, T1 arg1, T2 arg2, T3 arg3) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3);

			public void Warn<T1, T2, T3, T4>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4);

			public void Warn<T1, T2, T3, T4, T5>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5);

			public void Warn<T1, T2, T3, T4, T5, T6>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6);

			public void Warn<T1, T2, T3, T4, T5, T6, T7>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);

			public void Warn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) 
			=> Write(LogLevel.Warning, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);

			#endregion

		#region Error (Interface)

		public void Error(string message)
			=> Write(LogLevel.Error, Name, message);

			public void Error<T1>(string format, T1 arg1) 
			=> Write(LogLevel.Error, Name, format, arg1);

			public void Error<T1, T2>(string format, T1 arg1, T2 arg2) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2);

			public void Error<T1, T2, T3>(string format, T1 arg1, T2 arg2, T3 arg3) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3);

			public void Error<T1, T2, T3, T4>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4);

			public void Error<T1, T2, T3, T4, T5>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5);

			public void Error<T1, T2, T3, T4, T5, T6>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6);

			public void Error<T1, T2, T3, T4, T5, T6, T7>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);

			public void Error<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) 
			=> Write(LogLevel.Error, Name, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);

			#endregion

		#region Warning (Exception)

		public void Exception(Exception ex)
			=> WriteException(LogLevel.Exception, Name, ex);

		public void Exception(Exception ex, string message)
			=> WriteException(LogLevel.Exception, Name, ex, message);

			public void Exception<T1>(Exception ex, string format, T1 arg1) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1);

			public void Exception<T1, T2>(Exception ex, string format, T1 arg1, T2 arg2) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2);

			public void Exception<T1, T2, T3>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3);

			public void Exception<T1, T2, T3, T4>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4);

			public void Exception<T1, T2, T3, T4, T5>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5);

			public void Exception<T1, T2, T3, T4, T5, T6>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6);

			public void Exception<T1, T2, T3, T4, T5, T6, T7>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);

			public void Exception<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Exception ex, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) 
			=> WriteException(LogLevel.Exception, Name, ex, format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);

			#endregion
	}
}