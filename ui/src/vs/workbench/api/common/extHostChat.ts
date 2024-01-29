/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { CancellationToken } from 'vs/base/common/cancellation';
import { Emitter } from 'vs/base/common/event';
import { IMarkdownString } from 'vs/base/common/htmlContent';
import { Iterable } from 'vs/base/common/iterator';
import { toDisposable } from 'vs/base/common/lifecycle';
import { IRelaxedExtensionDescription } from 'vs/platform/extensions/common/extensions';
import { ExtHostChatShape, IChatDto, IMainContext, MainContext, MainThreadChatShape } from 'vs/workbench/api/common/extHost.protocol';
import * as typeConvert from 'vs/workbench/api/common/extHostTypeConverters';
import { IChatReplyFollowup, IChatUserActionEvent } from 'vs/workbench/contrib/chat/common/chatService';
import type * as vscode from 'vscode';

class ChatProviderWrapper<T> {

	private static _pool = 0;

	readonly handle: number = ChatProviderWrapper._pool++;

	constructor(
		readonly extension: Readonly<IRelaxedExtensionDescription>,
		readonly provider: T,
	) { }
}

export class ExtHostChat implements ExtHostChatShape {
	private static _nextId = 0;

	private readonly _chatProvider = new Map<number, ChatProviderWrapper<vscode.InteractiveSessionProvider>>();

	private readonly _chatSessions = new Map<number, vscode.InteractiveSession>();

	private readonly _onDidPerformUserAction = new Emitter<vscode.InteractiveSessionUserActionEvent>();
	public readonly onDidPerformUserAction = this._onDidPerformUserAction.event;

	private readonly _proxy: MainThreadChatShape;

	constructor(
		mainContext: IMainContext,
	) {
		this._proxy = mainContext.getProxy(MainContext.MainThreadChat);
	}

	//#region interactive session

	registerChatProvider(extension: Readonly<IRelaxedExtensionDescription>, id: string, provider: vscode.InteractiveSessionProvider): vscode.Disposable {
		const wrapper = new ChatProviderWrapper(extension, provider);
		this._chatProvider.set(wrapper.handle, wrapper);
		this._proxy.$registerChatProvider(wrapper.handle, id);
		return toDisposable(() => {
			this._proxy.$unregisterChatProvider(wrapper.handle);
			this._chatProvider.delete(wrapper.handle);
		});
	}

	transferChatSession(session: vscode.InteractiveSession, newWorkspace: vscode.Uri): void {
		const sessionId = Iterable.find(this._chatSessions.keys(), key => this._chatSessions.get(key) === session) ?? 0;
		if (typeof sessionId !== 'number') {
			return;
		}

		this._proxy.$transferChatSession(sessionId, newWorkspace);
	}

	sendInteractiveRequestToProvider(providerId: string, message: vscode.InteractiveSessionDynamicRequest): void {
		this._proxy.$sendRequestToProvider(providerId, message);
	}

	async $prepareChat(handle: number, token: CancellationToken): Promise<IChatDto | undefined> {
		const entry = this._chatProvider.get(handle);
		if (!entry) {
			return undefined;
		}

		const session = await entry.provider.prepareSession(token);
		if (!session) {
			return undefined;
		}

		const id = ExtHostChat._nextId++;
		this._chatSessions.set(id, session);

		return {
			id,
			requesterUsername: session.requester?.name,
			requesterAvatarIconUri: session.requester?.icon,
			responderUsername: session.responder?.name,
			responderAvatarIconUri: session.responder?.icon,
			inputPlaceholder: session.inputPlaceholder,
		};
	}

	async $provideWelcomeMessage(handle: number, token: CancellationToken): Promise<(string | IMarkdownString | IChatReplyFollowup[])[] | undefined> {
		const entry = this._chatProvider.get(handle);
		if (!entry) {
			return undefined;
		}

		if (!entry.provider.provideWelcomeMessage) {
			return undefined;
		}

		const content = await entry.provider.provideWelcomeMessage(token);
		if (!content) {
			return undefined;
		}
		return content.map(item => {
			if (typeof item === 'string') {
				return item;
			} else if (Array.isArray(item)) {
				return item.map(f => typeConvert.ChatReplyFollowup.from(f));
			} else {
				return typeConvert.MarkdownString.from(item);
			}
		});
	}

	async $provideSampleQuestions(handle: number, token: CancellationToken): Promise<IChatReplyFollowup[] | undefined> {
		const entry = this._chatProvider.get(handle);
		if (!entry) {
			return undefined;
		}

		if (!entry.provider.provideSampleQuestions) {
			return undefined;
		}

		const rawFollowups = await entry.provider.provideSampleQuestions(token);
		if (!rawFollowups) {
			return undefined;
		}

		return rawFollowups?.map(f => typeConvert.ChatReplyFollowup.from(f));
	}

	$releaseSession(sessionId: number) {
		this._chatSessions.delete(sessionId);
	}

	async $onDidPerformUserAction(event: IChatUserActionEvent): Promise<void> {
		this._onDidPerformUserAction.fire(event as any);
	}

	//#endregion
}
