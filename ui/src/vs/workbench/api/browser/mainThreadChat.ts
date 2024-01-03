/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Emitter } from 'vs/base/common/event';
import { Disposable, DisposableMap } from 'vs/base/common/lifecycle';
import { URI, UriComponents } from 'vs/base/common/uri';
import { ExtHostChatShape, ExtHostContext, MainContext, MainThreadChatShape } from 'vs/workbench/api/common/extHost.protocol';
import { IChatWidgetService } from 'vs/workbench/contrib/chat/browser/chat';
import { IChatContributionService } from 'vs/workbench/contrib/chat/common/chatContributionService';
import { IChatDynamicRequest, IChatService } from 'vs/workbench/contrib/chat/common/chatService';
import { IExtHostContext, extHostNamedCustomer } from 'vs/workbench/services/extensions/common/extHostCustomers';

@extHostNamedCustomer(MainContext.MainThreadChat)
export class MainThreadChat extends Disposable implements MainThreadChatShape {

	private readonly _providerRegistrations = this._register(new DisposableMap<number>());
	private readonly _stateEmitters = new Map<number, Emitter<any>>();

	private readonly _proxy: ExtHostChatShape;

	constructor(
		extHostContext: IExtHostContext,
		@IChatService private readonly _chatService: IChatService,
		@IChatWidgetService private readonly _chatWidgetService: IChatWidgetService,
		@IChatContributionService private readonly chatContribService: IChatContributionService,
	) {
		super();
		this._proxy = extHostContext.getProxy(ExtHostContext.ExtHostChat);

		this._register(this._chatService.onDidPerformUserAction(e => {
			if (!e.agentId) {
				this._proxy.$onDidPerformUserAction(e);
			}
		}));
	}

	$transferChatSession(sessionId: number, toWorkspace: UriComponents): void {
		const sessionIdStr = this._chatService.getSessionId(sessionId);
		if (!sessionIdStr) {
			throw new Error(`Failed to transfer session. Unknown session provider ID: ${sessionId}`);
		}

		const widget = this._chatWidgetService.getWidgetBySessionId(sessionIdStr);
		const inputValue = widget?.inputEditor.getValue() ?? '';
		this._chatService.transferChatSession({ sessionId: sessionIdStr, inputValue: inputValue }, URI.revive(toWorkspace));
	}

	async $registerChatProvider(handle: number, id: string): Promise<void> {
		const registration = this.chatContribService.registeredProviders.find(staticProvider => staticProvider.id === id);
		if (!registration) {
			throw new Error(`Provider ${id} must be declared in the package.json.`);
		}

		const unreg = this._chatService.registerProvider({
			id,
			displayName: registration.label,
			prepareSession: async (token) => {
				const session = await this._proxy.$prepareChat(handle, token);
				if (!session) {
					return undefined;
				}

				const responderAvatarIconUri = session.responderAvatarIconUri &&
					URI.revive(session.responderAvatarIconUri);

				const emitter = new Emitter<any>();
				this._stateEmitters.set(session.id, emitter);
				return {
					id: session.id,
					requesterUsername: session.requesterUsername,
					requesterAvatarIconUri: URI.revive(session.requesterAvatarIconUri),
					responderUsername: session.responderUsername,
					responderAvatarIconUri,
					inputPlaceholder: session.inputPlaceholder,
					dispose: () => {
						emitter.dispose();
						this._stateEmitters.delete(session.id);
						this._proxy.$releaseSession(session.id);
					}
				};
			},
			provideWelcomeMessage: (token) => {
				return this._proxy.$provideWelcomeMessage(handle, token);
			},
			provideSampleQuestions: (token) => {
				return this._proxy.$provideSampleQuestions(handle, token);
			},
		});

		this._providerRegistrations.set(handle, unreg);
	}

	async $acceptChatState(sessionId: number, state: any): Promise<void> {
		this._stateEmitters.get(sessionId)?.fire(state);
	}

	async $sendRequestToProvider(providerId: string, message: IChatDynamicRequest): Promise<void> {
		const widget = await this._chatWidgetService.revealViewForProvider(providerId);
		if (widget && widget.viewModel) {
			this._chatService.sendRequestToProvider(widget.viewModel.sessionId, message);
		}
	}

	async $unregisterChatProvider(handle: number): Promise<void> {
		this._providerRegistrations.deleteAndDispose(handle);
	}
}
