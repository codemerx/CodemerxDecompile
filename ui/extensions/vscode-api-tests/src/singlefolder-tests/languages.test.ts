/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as assert from 'assert';
import { join } from 'path';
import * as vscode from 'vscode';
import { assertNoRpc, createRandomFile, testFs } from '../utils';

suite('vscode API - languages', () => {

	teardown(assertNoRpc);

	const isWindows = process.platform === 'win32';

	function positionToString(p: vscode.Position) {
		return `[${p.character}/${p.line}]`;
	}

	function rangeToString(r: vscode.Range) {
		return `[${positionToString(r.start)}/${positionToString(r.end)}]`;
	}

	function assertEqualRange(actual: vscode.Range, expected: vscode.Range, message?: string) {
		assert.strictEqual(rangeToString(actual), rangeToString(expected), message);
	}

	test('setTextDocumentLanguage -> close/open event', async function () {
		const file = await createRandomFile('foo\nbar\nbar');
		const doc = await vscode.workspace.openTextDocument(file);
		const langIdNow = doc.languageId;
		let clock = 0;
		const disposables: vscode.Disposable[] = [];

		const close = new Promise<void>(resolve => {
			disposables.push(vscode.workspace.onDidCloseTextDocument(e => {
				if (e === doc) {
					assert.strictEqual(doc.languageId, langIdNow);
					assert.strictEqual(clock, 0);
					clock += 1;
					resolve();
				}
			}));
		});
		const open = new Promise<void>(resolve => {
			disposables.push(vscode.workspace.onDidOpenTextDocument(e => {
				if (e === doc) { // same instance!
					assert.strictEqual(doc.languageId, 'json');
					assert.strictEqual(clock, 1);
					clock += 1;
					resolve();
				}
			}));
		});
		const change = vscode.languages.setTextDocumentLanguage(doc, 'json');
		await Promise.all([change, close, open]);
		assert.strictEqual(clock, 2);
		assert.strictEqual(doc.languageId, 'json');
		disposables.forEach(disposable => disposable.dispose());
		disposables.length = 0;
	});

	test('setTextDocumentLanguage -> error when language does not exist', async function () {
		const file = await createRandomFile('foo\nbar\nbar');
		const doc = await vscode.workspace.openTextDocument(file);

		try {
			await vscode.languages.setTextDocumentLanguage(doc, 'fooLangDoesNotExist');
			assert.ok(false);
		} catch (err) {
			assert.ok(err);
		}
	});

	test('diagnostics, read & event', function () {
		const uri = vscode.Uri.file('/foo/bar.txt');
		const col1 = vscode.languages.createDiagnosticCollection('foo1');
		col1.set(uri, [new vscode.Diagnostic(new vscode.Range(0, 0, 0, 12), 'error1')]);

		const col2 = vscode.languages.createDiagnosticCollection('foo2');
		col2.set(uri, [new vscode.Diagnostic(new vscode.Range(0, 0, 0, 12), 'error1')]);

		const diag = vscode.languages.getDiagnostics(uri);
		assert.strictEqual(diag.length, 2);

		const tuples = vscode.languages.getDiagnostics();
		let found = false;
		for (const [thisUri,] of tuples) {
			if (thisUri.toString() === uri.toString()) {
				found = true;
				break;
			}
		}
		assert.ok(tuples.length >= 1);
		assert.ok(found);
	});

	// HINT: If this test fails, and you have been modifying code used in workers, you might have
	// accidentally broken the workers. Check the logs for errors.
	test('link detector', async function () {
		const uri = await createRandomFile('class A { // http://a.com }', undefined, '.java');
		const doc = await vscode.workspace.openTextDocument(uri);

		const target = vscode.Uri.file(isWindows ? 'c:\\foo\\bar' : '/foo/bar');
		const range = new vscode.Range(new vscode.Position(0, 0), new vscode.Position(0, 5));

		const linkProvider: vscode.DocumentLinkProvider = {
			provideDocumentLinks: _doc => {
				return [new vscode.DocumentLink(range, target)];
			}
		};
		vscode.languages.registerDocumentLinkProvider({ language: 'java', scheme: testFs.scheme }, linkProvider);

		const links = await vscode.commands.executeCommand<vscode.DocumentLink[]>('vscode.executeLinkProvider', doc.uri);
		assert.strictEqual(links && links.length, 2, links.map(l => !l.target).join(', '));
		const [link1, link2] = links!.sort((l1, l2) => l1.range.start.compareTo(l2.range.start));

		assert.strictEqual(link1.target && link1.target.toString(), target.toString());
		assertEqualRange(link1.range, range);

		assert.strictEqual(link2.target && link2.target.toString(), 'http://a.com/');
		assertEqualRange(link2.range, new vscode.Range(new vscode.Position(0, 13), new vscode.Position(0, 25)));
	});

	test('diagnostics & CodeActionProvider', async function () {

		class D2 extends vscode.Diagnostic {
			customProp = { complex() { } };
			constructor() {
				super(new vscode.Range(0, 2, 0, 7), 'sonntag');
			}
		}

		const diag1 = new vscode.Diagnostic(new vscode.Range(0, 0, 0, 5), 'montag');
		const diag2 = new D2();

		let ran = false;
		const uri = vscode.Uri.parse('ttt:path.far');

		const r1 = vscode.languages.registerCodeActionsProvider({ pattern: '*.far', scheme: 'ttt' }, {
			provideCodeActions(_document, _range, ctx): vscode.Command[] {

				assert.strictEqual(ctx.diagnostics.length, 2);
				const [first, second] = ctx.diagnostics;
				assert.ok(first === diag1);
				assert.ok(second === diag2);
				assert.ok(diag2 instanceof D2);
				ran = true;
				return [];
			}
		});

		const r2 = vscode.workspace.registerTextDocumentContentProvider('ttt', {
			provideTextDocumentContent() {
				return 'this is some text';
			}
		});

		const r3 = vscode.languages.createDiagnosticCollection();
		r3.set(uri, [diag1]);

		const r4 = vscode.languages.createDiagnosticCollection();
		r4.set(uri, [diag2]);

		await vscode.workspace.openTextDocument(uri);
		await vscode.commands.executeCommand('vscode.executeCodeActionProvider', uri, new vscode.Range(0, 0, 0, 10));
		assert.ok(ran);
		vscode.Disposable.from(r1, r2, r3, r4).dispose();
	});

	test('completions with document filters', async function () {
		let ran = false;
		const uri = vscode.Uri.file(join(vscode.workspace.rootPath || '', './bower.json'));

		const jsonDocumentFilter = [{ language: 'json', pattern: '**/package.json' }, { language: 'json', pattern: '**/bower.json' }, { language: 'json', pattern: '**/.bower.json' }];

		const r1 = vscode.languages.registerCompletionItemProvider(jsonDocumentFilter, {
			provideCompletionItems: (_document: vscode.TextDocument, _position: vscode.Position, _token: vscode.CancellationToken): vscode.CompletionItem[] => {
				const proposal = new vscode.CompletionItem('foo');
				proposal.kind = vscode.CompletionItemKind.Property;
				ran = true;
				return [proposal];
			}
		});

		await vscode.workspace.openTextDocument(uri);
		const result = await vscode.commands.executeCommand<vscode.CompletionList>('vscode.executeCompletionItemProvider', uri, new vscode.Position(1, 0));
		r1.dispose();
		assert.ok(ran, 'Provider has not been invoked');
		assert.ok(result!.items.some(i => i.label === 'foo'), 'Results do not include "foo"');
	});

	test('folding command', async function () {
		const content = `[
			/**
			 * This is a comment with indentation issues
		*/
			{
				"name": "bag of items",
				"items": [
					"foo", "bar"
				]
			}
		]`;
		const uri = await createRandomFile(content, undefined, '.jsonc');
		await vscode.workspace.openTextDocument(uri);
		const jsonExtension = await vscode.extensions.getExtension('vscode.json-language-features');
		assert.ok(jsonExtension);
		await jsonExtension.activate();
		const result1 = await vscode.commands.executeCommand<vscode.FoldingRange[]>('vscode.executeFoldingRangeProvider', uri);
		assert.deepEqual(result1, [
			{ start: 0, end: 9 },
			{ start: 1, end: 3, kind: vscode.FoldingRangeKind.Comment },
			{ start: 4, end: 8 },
			{ start: 6, end: 7 },
		]);

		await vscode.workspace.getConfiguration('editor').update('foldingStrategy', 'indentation');
		try {
			const result2 = await vscode.commands.executeCommand<vscode.FoldingRange[]>('vscode.executeFoldingRangeProvider', uri);
			assert.deepEqual(result2, [
				{ start: 0, end: 10 },
				{ start: 1, end: 2 },
				{ start: 3, end: 9 },
				{ start: 4, end: 8 },
				{ start: 6, end: 7 },
			]);
			await vscode.workspace.getConfiguration('editor').update('folding', false);
			const result3 = await vscode.commands.executeCommand<vscode.FoldingRange[]>('vscode.executeFoldingRangeProvider', uri);
			assert.deepEqual(result3, []);
		} finally {
			await vscode.workspace.getConfiguration('editor').update('foldingStrategy', undefined);
			await vscode.workspace.getConfiguration('editor').update('folding', undefined);
		}
	});
});
