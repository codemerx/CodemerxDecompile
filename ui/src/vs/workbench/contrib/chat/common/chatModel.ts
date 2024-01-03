/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { asArray, firstOrDefault } from 'vs/base/common/arrays';
import { DeferredPromise } from 'vs/base/common/async';
import { Emitter, Event } from 'vs/base/common/event';
import { IMarkdownString, MarkdownString, isMarkdownString } from 'vs/base/common/htmlContent';
import { Disposable } from 'vs/base/common/lifecycle';
import { revive } from 'vs/base/common/marshalling';
import { basename } from 'vs/base/common/resources';
import { URI, UriComponents, UriDto } from 'vs/base/common/uri';
import { generateUuid } from 'vs/base/common/uuid';
import { OffsetRange } from 'vs/editor/common/core/offsetRange';
import { ILogService } from 'vs/platform/log/common/log';
import { IChatAgentCommand, IChatAgentData, IChatAgentService } from 'vs/workbench/contrib/chat/common/chatAgents';
import { ChatRequestTextPart, IParsedChatRequest, reviveParsedChatRequest } from 'vs/workbench/contrib/chat/common/chatParserTypes';
import { IChat, IChatAgentMarkdownContentWithVulnerability, IChatAsyncContent, IChatContent, IChatContentInlineReference, IChatContentReference, IChatFollowup, IChatMarkdownContent, IChatProgress, IChatProgressMessage, IChatReplyFollowup, IChatResponse, IChatResponseErrorDetails, IChatResponseProgressFileTreeData, IChatTreeData, IChatUsedContext, InteractiveSessionVoteDirection, isIUsedContext } from 'vs/workbench/contrib/chat/common/chatService';

export interface IChatRequestModel {
	readonly id: string;
	readonly username: string;
	readonly avatarIconUri?: URI;
	readonly session: IChatModel;
	readonly message: IParsedChatRequest | IChatReplyFollowup;
	readonly response: IChatResponseModel | undefined;
}

export type IChatProgressResponseContent =
	| IChatMarkdownContent
	| IChatAgentMarkdownContentWithVulnerability
	| IChatTreeData
	| IChatAsyncContent
	| IChatContentInlineReference;

export type IChatProgressRenderableResponseContent = Exclude<IChatProgressResponseContent, IChatContentInlineReference | IChatAgentMarkdownContentWithVulnerability>;

export interface IResponse {
	readonly value: ReadonlyArray<IChatProgressResponseContent>;
	asString(): string;
}

export interface IChatResponseModel {
	readonly onDidChange: Event<void>;
	readonly id: string;
	readonly providerId: string;
	readonly requestId: string;
	readonly username: string;
	readonly avatarIconUri?: URI;
	readonly session: IChatModel;
	readonly agent?: IChatAgentData;
	readonly usedContext: IChatUsedContext | undefined;
	readonly contentReferences: ReadonlyArray<IChatContentReference>;
	readonly progressMessages: ReadonlyArray<IChatProgressMessage>;
	readonly slashCommand?: IChatAgentCommand;
	readonly response: IResponse;
	readonly isComplete: boolean;
	readonly isCanceled: boolean;
	readonly vote: InteractiveSessionVoteDirection | undefined;
	readonly followups?: IChatFollowup[] | undefined;
	readonly errorDetails?: IChatResponseErrorDetails;
	setVote(vote: InteractiveSessionVoteDirection): void;
}

export class ChatRequestModel implements IChatRequestModel {
	private static nextId = 0;

	public response: ChatResponseModel | undefined;

	private _id: string;
	public get id(): string {
		return this._id;
	}

	public get username(): string {
		return this.session.requesterUsername;
	}

	public get avatarIconUri(): URI | undefined {
		return this.session.requesterAvatarIconUri;
	}

	constructor(
		public readonly session: ChatModel,
		public readonly message: IParsedChatRequest) {
		this._id = 'request_' + ChatRequestModel.nextId++;
	}
}

export class Response implements IResponse {
	private _onDidChangeValue = new Emitter<void>();
	public get onDidChangeValue() {
		return this._onDidChangeValue.event;
	}

	// responseParts internally tracks all the response parts, including strings which are currently resolving, so that they can be updated when they do resolve
	private _responseParts: IChatProgressResponseContent[];
	// responseRepr externally presents the response parts with consolidated contiguous strings (excluding tree data)
	private _responseRepr!: string;

	get value(): IChatProgressResponseContent[] {
		return this._responseParts;
	}

	constructor(value: IMarkdownString | ReadonlyArray<IMarkdownString | IChatResponseProgressFileTreeData | IChatContentInlineReference | IChatAgentMarkdownContentWithVulnerability>) {
		this._responseParts = asArray(value).map((v) => (isMarkdownString(v) ?
			{ content: v, kind: 'markdownContent' } satisfies IChatMarkdownContent :
			'kind' in v ? v : { kind: 'treeData', treeData: v }));

		this._updateRepr(true);
	}

	asString(): string {
		return this._responseRepr;
	}

	clear(): void {
		this._responseParts = [];
		this._updateRepr(true);
	}

	updateContent(progress: IChatProgressResponseContent | IChatContent, quiet?: boolean): void {
		if (progress.kind === 'content' || progress.kind === 'markdownContent') {
			const responsePartLength = this._responseParts.length - 1;
			const lastResponsePart = this._responseParts[responsePartLength];

			if (!lastResponsePart || lastResponsePart.kind !== 'markdownContent') {
				// The last part can't be merged with
				if (progress.kind === 'content') {
					this._responseParts.push({ content: new MarkdownString(progress.content), kind: 'markdownContent' });
				} else {
					this._responseParts.push(progress);
				}
			} else if (progress.kind === 'markdownContent') {
				// Merge all enabled commands
				const lastPartEnabledCommands = typeof lastResponsePart.content.isTrusted === 'object' ?
					lastResponsePart.content.isTrusted.enabledCommands :
					[];
				const thisPartEnabledCommands = typeof progress.content.isTrusted === 'object' ?
					progress.content.isTrusted.enabledCommands :
					[];
				const enabledCommands = [...lastPartEnabledCommands, ...thisPartEnabledCommands];
				this._responseParts[responsePartLength] = { content: new MarkdownString(lastResponsePart.content.value + progress.content.value, { isTrusted: { enabledCommands } }), kind: 'markdownContent' };
			} else {
				this._responseParts[responsePartLength] = { content: new MarkdownString(lastResponsePart.content.value + progress.content, lastResponsePart.content), kind: 'markdownContent' };
			}

			this._updateRepr(quiet);
		} else if (progress.kind === 'asyncContent') {
			// Add a new resolving part
			const responsePosition = this._responseParts.push(progress) - 1;
			this._updateRepr(quiet);

			progress.resolvedContent?.then((content) => {
				// Replace the resolving part's content with the resolved response
				if (typeof content === 'string') {
					this._responseParts[responsePosition] = { content: new MarkdownString(content), kind: 'markdownContent' };
				} else if (isMarkdownString(content)) {
					this._responseParts[responsePosition] = { content, kind: 'markdownContent' };
				} else {
					this._responseParts[responsePosition] = content;
				}
				this._updateRepr(quiet);
			});
		} else if (progress.kind === 'treeData' || progress.kind === 'inlineReference' || progress.kind === 'markdownVuln') {
			this._responseParts.push(progress);
			this._updateRepr(quiet);
		}
	}

	private _updateRepr(quiet?: boolean) {
		this._responseRepr = this._responseParts.map(part => {
			if (part.kind === 'treeData') {
				return '';
			} else if (part.kind === 'inlineReference') {
				return basename('uri' in part.inlineReference ? part.inlineReference.uri : part.inlineReference);
			} else if (part.kind === 'asyncContent') {
				return part.content;
			} else {
				return part.content.value;
			}
		}).join('\n\n');

		if (!quiet) {
			this._onDidChangeValue.fire();
		}
	}
}

export class ChatResponseModel extends Disposable implements IChatResponseModel {
	private readonly _onDidChange = this._register(new Emitter<void>());
	readonly onDidChange = this._onDidChange.event;

	private static nextId = 0;

	private _id: string;
	public get id(): string {
		return this._id;
	}

	public get isComplete(): boolean {
		return this._isComplete;
	}

	public get isCanceled(): boolean {
		return this._isCanceled;
	}

	public get vote(): InteractiveSessionVoteDirection | undefined {
		return this._vote;
	}

	public get followups(): IChatFollowup[] | undefined {
		return this._followups;
	}

	private _response: Response;
	public get response(): IResponse {
		return this._response;
	}

	public get errorDetails(): IChatResponseErrorDetails | undefined {
		return this._errorDetails;
	}

	public get providerId(): string {
		return this.session.providerId;
	}

	public get username(): string {
		return this.session.responderUsername;
	}

	public get avatarIconUri(): URI | undefined {
		return this.session.responderAvatarIconUri;
	}

	private _followups?: IChatFollowup[];

	private _agent: IChatAgentData | undefined;
	public get agent(): IChatAgentData | undefined {
		return this._agent;
	}

	private _slashCommand: IChatAgentCommand | undefined;
	public get slashCommand(): IChatAgentCommand | undefined {
		return this._slashCommand;
	}

	private _usedContext: IChatUsedContext | undefined;
	public get usedContext(): IChatUsedContext | undefined {
		return this._usedContext;
	}

	private readonly _contentReferences: IChatContentReference[] = [];
	public get contentReferences(): ReadonlyArray<IChatContentReference> {
		return this._contentReferences;
	}

	private readonly _progressMessages: IChatProgressMessage[] = [];
	public get progressMessages(): ReadonlyArray<IChatProgressMessage> {
		return this._progressMessages;
	}

	constructor(
		_response: IMarkdownString | ReadonlyArray<IMarkdownString | IChatResponseProgressFileTreeData | IChatContentInlineReference | IChatAgentMarkdownContentWithVulnerability>,
		public readonly session: ChatModel,
		agent: IChatAgentData | undefined,
		public readonly requestId: string,
		private _isComplete: boolean = false,
		private _isCanceled = false,
		private _vote?: InteractiveSessionVoteDirection,
		private _errorDetails?: IChatResponseErrorDetails,
		followups?: ReadonlyArray<IChatFollowup>
	) {
		super();
		this._agent = agent;
		this._followups = followups ? [...followups] : undefined;
		this._response = new Response(_response);
		this._register(this._response.onDidChangeValue(() => this._onDidChange.fire()));
		this._id = 'response_' + ChatResponseModel.nextId++;
	}

	/**
	 * Apply a progress update to the actual response content.
	 */
	updateContent(responsePart: IChatProgressResponseContent | IChatContent, quiet?: boolean) {
		this._response.updateContent(responsePart, quiet);
	}

	/**
	 * Apply one of the progress updates that are not part of the actual response content.
	 */
	applyProgress(progress: IChatUsedContext | IChatContentReference | IChatProgressMessage) {
		if (progress.kind === 'usedContext') {
			this._usedContext = progress;
		} else if (progress.kind === 'reference') {
			this._contentReferences.push(progress);
			this._onDidChange.fire();
		} else if (progress.kind === 'progressMessage') {
			this._progressMessages.push(progress);
			this._onDidChange.fire();
		}
	}

	setAgent(agent: IChatAgentData, slashCommand?: IChatAgentCommand) {
		this._agent = agent;
		this._slashCommand = slashCommand;
		this._onDidChange.fire();
	}

	setErrorDetails(errorDetails?: IChatResponseErrorDetails): void {
		this._errorDetails = errorDetails;
		this._onDidChange.fire();
	}

	complete(errorDetails?: IChatResponseErrorDetails): void {
		if (errorDetails?.responseIsRedacted) {
			this._response.clear();
		}

		this._isComplete = true;
		this._onDidChange.fire();
	}

	cancel(): void {
		this._isComplete = true;
		this._isCanceled = true;
		this._onDidChange.fire();
	}

	setFollowups(followups: IChatFollowup[] | undefined): void {
		this._followups = followups;
		this._onDidChange.fire(); // Fire so that command followups get rendered on the row
	}

	setVote(vote: InteractiveSessionVoteDirection): void {
		this._vote = vote;
		this._onDidChange.fire();
	}
}

export interface IChatModel {
	readonly onDidDispose: Event<void>;
	readonly onDidChange: Event<IChatChangeEvent>;
	readonly sessionId: string;
	readonly providerId: string;
	readonly initState: ChatModelInitState;
	readonly title: string;
	readonly welcomeMessage: IChatWelcomeMessageModel | undefined;
	readonly requestInProgress: boolean;
	readonly inputPlaceholder?: string;
	getRequests(): IChatRequestModel[];
	toExport(): IExportableChatData;
	toJSON(): ISerializableChatData;
}

export interface ISerializableChatsData {
	[sessionId: string]: ISerializableChatData;
}

export type ISerializableChatAgentData = UriDto<IChatAgentData>;

export interface ISerializableChatRequestData {
	message: string | IParsedChatRequest;
	response: ReadonlyArray<IMarkdownString | IChatResponseProgressFileTreeData | IChatContentInlineReference | IChatAgentMarkdownContentWithVulnerability> | undefined;
	agent?: ISerializableChatAgentData;
	slashCommand?: IChatAgentCommand;
	responseErrorDetails: IChatResponseErrorDetails | undefined;
	followups: ReadonlyArray<IChatFollowup> | undefined;
	isCanceled: boolean | undefined;
	vote: InteractiveSessionVoteDirection | undefined;
	/** For backward compat: should be optional */
	usedContext?: IChatUsedContext;
	contentReferences?: ReadonlyArray<IChatContentReference>;
}

export interface IExportableChatData {
	providerId: string;
	welcomeMessage: (string | IChatReplyFollowup[])[] | undefined;
	requests: ISerializableChatRequestData[];
	requesterUsername: string;
	responderUsername: string;
	requesterAvatarIconUri: UriComponents | undefined;
	responderAvatarIconUri: UriComponents | undefined;
}

export interface ISerializableChatData extends IExportableChatData {
	sessionId: string;
	creationDate: number;
	isImported: boolean;
}

export function isExportableSessionData(obj: unknown): obj is IExportableChatData {
	const data = obj as IExportableChatData;
	return typeof data === 'object' &&
		typeof data.providerId === 'string' &&
		typeof data.requesterUsername === 'string' &&
		typeof data.responderUsername === 'string';
}

export function isSerializableSessionData(obj: unknown): obj is ISerializableChatData {
	const data = obj as ISerializableChatData;
	return isExportableSessionData(obj) &&
		typeof data.creationDate === 'number' &&
		typeof data.sessionId === 'string' &&
		obj.requests.every((request: ISerializableChatRequestData) =>
			!request.usedContext /* for backward compat allow missing usedContext */ || isIUsedContext(request.usedContext)
		);
}

export type IChatChangeEvent = IChatAddRequestEvent | IChatAddResponseEvent | IChatInitEvent | IChatRemoveRequestEvent;

export interface IChatAddRequestEvent {
	kind: 'addRequest';
	request: IChatRequestModel;
}

export interface IChatAddResponseEvent {
	kind: 'addResponse';
	response: IChatResponseModel;
}

export interface IChatRemoveRequestEvent {
	kind: 'removeRequest';
	requestId: string;
	responseId?: string;
}

export interface IChatInitEvent {
	kind: 'initialize';
}

export enum ChatModelInitState {
	Created,
	Initializing,
	Initialized
}

export class ChatModel extends Disposable implements IChatModel {
	private readonly _onDidDispose = this._register(new Emitter<void>());
	readonly onDidDispose = this._onDidDispose.event;

	private readonly _onDidChange = this._register(new Emitter<IChatChangeEvent>());
	readonly onDidChange = this._onDidChange.event;

	private _requests: ChatRequestModel[];
	private _initState: ChatModelInitState = ChatModelInitState.Created;
	private _isInitializedDeferred = new DeferredPromise<void>();

	private _session: IChat | undefined;
	get session(): IChat | undefined {
		return this._session;
	}

	private _welcomeMessage: ChatWelcomeMessageModel | undefined;
	get welcomeMessage(): ChatWelcomeMessageModel | undefined {
		return this._welcomeMessage;
	}

	// TODO to be clear, this is not the same as the id from the session object, which belongs to the provider.
	// It's easier to be able to identify this model before its async initialization is complete
	private _sessionId: string;
	get sessionId(): string {
		return this._sessionId;
	}

	get inputPlaceholder(): string | undefined {
		return this._session?.inputPlaceholder;
	}

	get requestInProgress(): boolean {
		const lastRequest = this._requests[this._requests.length - 1];
		return !!lastRequest && !!lastRequest.response && !lastRequest.response.isComplete;
	}

	private _creationDate: number;
	get creationDate(): number {
		return this._creationDate;
	}

	get requesterUsername(): string {
		return this._session?.requesterUsername ?? this.initialData?.requesterUsername ?? '';
	}

	get responderUsername(): string {
		return this._session?.responderUsername ?? this.initialData?.responderUsername ?? '';
	}

	private readonly _initialRequesterAvatarIconUri: URI | undefined;
	get requesterAvatarIconUri(): URI | undefined {
		return this._session ? this._session.requesterAvatarIconUri : this._initialRequesterAvatarIconUri;
	}

	private readonly _initialResponderAvatarIconUri: URI | undefined;
	get responderAvatarIconUri(): URI | undefined {
		return this._session ? this._session.responderAvatarIconUri : this._initialResponderAvatarIconUri;
	}

	get initState(): ChatModelInitState {
		return this._initState;
	}

	private _isImported = false;
	get isImported(): boolean {
		return this._isImported;
	}

	get title(): string {
		const firstRequestMessage = firstOrDefault(this._requests)?.message;
		const message = firstRequestMessage?.text ?? '';
		return message.split('\n')[0].substring(0, 50);
	}

	constructor(
		public readonly providerId: string,
		private readonly initialData: ISerializableChatData | IExportableChatData | undefined,
		@ILogService private readonly logService: ILogService,
		@IChatAgentService private readonly chatAgentService: IChatAgentService,
	) {
		super();

		this._isImported = (!!initialData && !isSerializableSessionData(initialData)) || (initialData?.isImported ?? false);
		this._sessionId = (isSerializableSessionData(initialData) && initialData.sessionId) || generateUuid();
		this._requests = initialData ? this._deserialize(initialData) : [];
		this._creationDate = (isSerializableSessionData(initialData) && initialData.creationDate) || Date.now();

		this._initialRequesterAvatarIconUri = initialData?.requesterAvatarIconUri && URI.revive(initialData.requesterAvatarIconUri);
		this._initialResponderAvatarIconUri = initialData?.responderAvatarIconUri && URI.revive(initialData.responderAvatarIconUri);
	}

	private _deserialize(obj: IExportableChatData): ChatRequestModel[] {
		const requests = obj.requests;
		if (!Array.isArray(requests)) {
			this.logService.error(`Ignoring malformed session data: ${JSON.stringify(obj)}`);
			return [];
		}

		if (obj.welcomeMessage) {
			const content = obj.welcomeMessage.map(item => typeof item === 'string' ? new MarkdownString(item) : item);
			this._welcomeMessage = new ChatWelcomeMessageModel(this, content, []);
		}

		try {
			return requests.map((raw: ISerializableChatRequestData) => {
				const parsedRequest =
					typeof raw.message === 'string'
						? this.getParsedRequestFromString(raw.message)
						: reviveParsedChatRequest(raw.message);
				const request = new ChatRequestModel(this, parsedRequest);
				if (raw.response || raw.responseErrorDetails) {
					const agent = (raw.agent && 'metadata' in raw.agent) ? // Check for the new format, ignore entries in the old format
						revive<ISerializableChatAgentData>(raw.agent) : undefined;
					request.response = new ChatResponseModel(raw.response ?? [new MarkdownString(raw.response)], this, agent, request.id, true, raw.isCanceled, raw.vote, raw.responseErrorDetails, raw.followups);
					if (raw.usedContext) { // @ulugbekna: if this's a new vscode sessions, doc versions are incorrect anyway?
						request.response.applyProgress(raw.usedContext);
					}

					if (raw.contentReferences) {
						raw.contentReferences.forEach(r => request.response!.applyProgress(r));
					}
				}
				return request;
			});
		} catch (error) {
			this.logService.error('Failed to parse chat data', error);
			return [];
		}
	}

	private getParsedRequestFromString(message: string): IParsedChatRequest {
		// TODO These offsets won't be used, but chat replies need to go through the parser as well
		const parts = [new ChatRequestTextPart(new OffsetRange(0, message.length), { startColumn: 1, startLineNumber: 1, endColumn: 1, endLineNumber: 1 }, message)];
		return {
			text: message,
			parts
		};
	}

	startInitialize(): void {
		if (this.initState !== ChatModelInitState.Created) {
			throw new Error(`ChatModel is in the wrong state for startInitialize: ${ChatModelInitState[this.initState]}`);
		}
		this._initState = ChatModelInitState.Initializing;
	}

	deinitialize(): void {
		this._session = undefined;
		this._initState = ChatModelInitState.Created;
		this._isInitializedDeferred = new DeferredPromise<void>();
	}

	initialize(session: IChat, welcomeMessage: ChatWelcomeMessageModel | undefined): void {
		if (this.initState !== ChatModelInitState.Initializing) {
			// Must call startInitialize before initialize, and only call it once
			throw new Error(`ChatModel is in the wrong state for initialize: ${ChatModelInitState[this.initState]}`);
		}

		this._initState = ChatModelInitState.Initialized;
		this._session = session;
		if (!this._welcomeMessage) {
			// Could also have loaded the welcome message from persisted data
			this._welcomeMessage = welcomeMessage;
		}

		this._isInitializedDeferred.complete();
		this._onDidChange.fire({ kind: 'initialize' });
	}

	setInitializationError(error: Error): void {
		if (this.initState !== ChatModelInitState.Initializing) {
			throw new Error(`ChatModel is in the wrong state for setInitializationError: ${ChatModelInitState[this.initState]}`);
		}

		if (!this._isInitializedDeferred.isSettled) {
			this._isInitializedDeferred.error(error);
		}
	}

	waitForInitialization(): Promise<void> {
		return this._isInitializedDeferred.p;
	}

	getRequests(): ChatRequestModel[] {
		return this._requests;
	}

	addRequest(message: IParsedChatRequest, chatAgent?: IChatAgentData): ChatRequestModel {
		if (!this._session) {
			throw new Error('addRequest: No session');
		}

		const request = new ChatRequestModel(this, message);
		request.response = new ChatResponseModel([], this, chatAgent, request.id);

		this._requests.push(request);
		this._onDidChange.fire({ kind: 'addRequest', request });
		return request;
	}

	acceptResponseProgress(request: ChatRequestModel, progress: IChatProgress, quiet?: boolean): void {
		if (!this._session) {
			throw new Error('acceptResponseProgress: No session');
		}

		if (!request.response) {
			request.response = new ChatResponseModel([], this, undefined, request.id);
		}

		if (request.response.isComplete) {
			throw new Error('acceptResponseProgress: Adding progress to a completed response');
		}

		if (progress.kind === 'vulnerability') {
			// TODO@roblourens ChatModel should just work with strings
			request.response.updateContent({ kind: 'markdownVuln', content: { value: progress.content }, vulnerabilities: progress.vulnerabilities }, quiet);
		} else if (progress.kind === 'content' || progress.kind === 'markdownContent' || progress.kind === 'asyncContent' || progress.kind === 'treeData' || progress.kind === 'inlineReference' || progress.kind === 'markdownVuln') {
			request.response.updateContent(progress, quiet);
		} else if (progress.kind === 'usedContext' || progress.kind === 'reference' || progress.kind === 'progressMessage') {
			request.response.applyProgress(progress);
		} else if (progress.kind === 'agentDetection') {
			const agent = this.chatAgentService.getAgent(progress.agentName);
			if (agent) {
				request.response.setAgent(agent, progress.command);
			}
		} else {
			this.logService.error(`Couldn't handle progress: ${JSON.stringify(progress)}`);
		}
	}

	removeRequest(id: string): void {
		const index = this._requests.findIndex(request => request.id === id);
		const request = this._requests[index];

		if (index !== -1) {
			this._onDidChange.fire({ kind: 'removeRequest', requestId: request.id, responseId: request.response?.id });
			this._requests.splice(index, 1);
			request.response?.dispose();
		}
	}

	cancelRequest(request: ChatRequestModel): void {
		if (request.response) {
			request.response.cancel();
		}
	}

	setResponse(request: ChatRequestModel, rawResponse: IChatResponse): void {
		if (!this._session) {
			throw new Error('completeResponse: No session');
		}

		if (!request.response) {
			request.response = new ChatResponseModel([], this, undefined, request.id);
		}

		request.response.setErrorDetails(rawResponse.errorDetails);
	}

	completeResponse(request: ChatRequestModel, errorDetails: IChatResponseErrorDetails | undefined): void {
		if (!request.response) {
			throw new Error('Call setResponse before completeResponse');
		}

		request.response.complete(errorDetails);
	}

	setFollowups(request: ChatRequestModel, followups: IChatFollowup[] | undefined): void {
		if (!request.response) {
			// Maybe something went wrong?
			return;
		}

		request.response.setFollowups(followups);
	}

	setResponseModel(request: ChatRequestModel, response: ChatResponseModel): void {
		request.response = response;
		this._onDidChange.fire({ kind: 'addResponse', response });
	}

	toExport(): IExportableChatData {
		return {
			requesterUsername: this.requesterUsername,
			requesterAvatarIconUri: this.requesterAvatarIconUri,
			responderUsername: this.responderUsername,
			responderAvatarIconUri: this.responderAvatarIconUri,
			welcomeMessage: this._welcomeMessage?.content.map(c => {
				if (Array.isArray(c)) {
					return c;
				} else {
					return c.value;
				}
			}),
			requests: this._requests.map((r): ISerializableChatRequestData => {
				return {
					message: r.message,
					response: r.response ?
						r.response.response.value.map(item => {
							// Keeping the shape of the persisted data the same for back compat
							if (item.kind === 'treeData') {
								return item.treeData;
							} else if (item.kind === 'markdownContent') {
								return item.content;
							} else if (item.kind === 'asyncContent') {
								return new MarkdownString(item.content);
							} else {
								return item;
							}
						})
						: undefined,
					responseErrorDetails: r.response?.errorDetails,
					followups: r.response?.followups,
					isCanceled: r.response?.isCanceled,
					vote: r.response?.vote,
					agent: r.response?.agent ? { id: r.response.agent.id, metadata: r.response.agent.metadata } : undefined, // May actually be the full IChatAgent instance, just take the data props
					slashCommand: r.response?.slashCommand,
					usedContext: r.response?.usedContext,
					contentReferences: r.response?.contentReferences
				};
			}),
			providerId: this.providerId,
		};
	}

	toJSON(): ISerializableChatData {
		return {
			...this.toExport(),
			sessionId: this.sessionId,
			creationDate: this._creationDate,
			isImported: this._isImported
		};
	}

	override dispose() {
		this._session?.dispose?.();
		this._requests.forEach(r => r.response?.dispose());
		this._onDidDispose.fire();

		super.dispose();
	}
}

export type IChatWelcomeMessageContent = IMarkdownString | IChatReplyFollowup[];

export interface IChatWelcomeMessageModel {
	readonly id: string;
	readonly content: IChatWelcomeMessageContent[];
	readonly sampleQuestions: IChatReplyFollowup[];
	readonly username: string;
	readonly avatarIconUri?: URI;

}

export class ChatWelcomeMessageModel implements IChatWelcomeMessageModel {
	private static nextId = 0;

	private _id: string;
	public get id(): string {
		return this._id;
	}

	constructor(
		private readonly session: ChatModel,
		public readonly content: IChatWelcomeMessageContent[],
		public readonly sampleQuestions: IChatReplyFollowup[]
	) {
		this._id = 'welcome_' + ChatWelcomeMessageModel.nextId++;
	}

	public get username(): string {
		return this.session.responderUsername;
	}

	public get avatarIconUri(): URI | undefined {
		return this.session.responderAvatarIconUri;
	}
}
