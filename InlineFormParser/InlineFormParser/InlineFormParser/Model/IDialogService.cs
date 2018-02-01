using Microsoft.Win32;
using System;
using InlineFormParser.Common.ResourceAccess;

namespace InlineFormParser.Model
{

	public interface IDialogService
	{
		bool IsAnyDialogActive
		{
			get;
		}

		void AbortAll();

		void BeginBusy(bool showBoxImmediately);

		void EndBusy();

		object ModalDialog(DialogBase dialog, IResourceAccessor resourceAccessor, string captionResourceKey);

		DialogAnswer ModalDialog(FileDialog dialog, IResourceAccessor resourceAccessor, string captionResourceKey);

		MessageBoxButtons ModalMessageBox(IResourceAccessor resourceAccessor, string captionResourceKey, MessageBoxSymbol symbol, MessageBoxButtons buttons, string messageResourceKey, params object[] messageParams);

		DialogAnswer ModalMessageBox(IResourceAccessor resourceAccessor, string captionResourceKey, MessageBoxSymbol symbol, DialogAnswer[] dialogAnswers, string messageResourceKey, params object[] messageParams);

		IDialogHandler OpenDialog(DialogBase dialog, IResourceAccessor resourceAccessor, string captionResourceKey, Action<DialogBase, object> closedCallback);

		IDialogHandler OpenMessageBox(IResourceAccessor resourceAccessor, string captionResourceKey, MessageBoxSymbol symbol, MessageBoxButtons buttons, Action<DialogBase, object> closedCallback, string messageResourceKey, params object[] messageParams);

		IDialogHandler OpenMessageBox(IResourceAccessor resourceAccessor, string captionResourceKey, MessageBoxSymbol symbol, DialogAnswer[] dialogAnswers, Action<DialogAnswer> closedCallback, string messageResourceKey, params object[] messageParams);
	}
}