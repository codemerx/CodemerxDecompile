/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { h } from 'vs/base/browser/dom';
import { assertNever } from 'vs/base/common/assert';
import { IMarkdownString, MarkdownString } from 'vs/base/common/htmlContent';
import { Lazy } from 'vs/base/common/lazy';
import { Disposable, DisposableStore, toDisposable } from 'vs/base/common/lifecycle';
import { ITransaction, autorun, observableFromEvent, observableValue } from 'vs/base/common/observable';
import { isDefined } from 'vs/base/common/types';
import { URI } from 'vs/base/common/uri';
import { localize } from 'vs/nls';
import { IConfigurationService } from 'vs/platform/configuration/common/configuration';
import { asCssVariableName, chartsGreen, chartsRed, chartsYellow } from 'vs/platform/theme/common/colorRegistry';
import { IExplorerFileContribution } from 'vs/workbench/contrib/files/browser/explorerFileContrib';
import { TestingConfigKeys, TestingDisplayedCoveragePercent, getTestingConfiguration } from 'vs/workbench/contrib/testing/common/configuration';
import { AbstractFileCoverage } from 'vs/workbench/contrib/testing/common/testCoverage';
import { ITestCoverageService } from 'vs/workbench/contrib/testing/common/testCoverageService';
import { ICoveredCount } from 'vs/workbench/contrib/testing/common/testTypes';
import { IHoverService } from 'vs/workbench/services/hover/browser/hover';

export interface TestCoverageBarsOptions {
	/**
	 * Whether the bars should be shown in a more compact way, where only the
	 * overall bar is shown and more details are given in the hover.
	 */
	compact: boolean;
	/**
	 * Container in which is render the bars.
	 */
	container: HTMLElement;
}

const colorThresholds = [
	{ color: asCssVariableName(chartsGreen), threshold: 0.9 },
	{ color: asCssVariableName(chartsYellow), threshold: 0.6 },
	{ color: asCssVariableName(chartsRed), threshold: -Infinity },
];

export class ManagedTestCoverageBars extends Disposable {
	private _coverage?: AbstractFileCoverage;
	private readonly el = new Lazy(() => {
		if (this.options.compact) {
			const el = h('.test-coverage-bars.compact', [
				h('.tpc@overall'),
				h('.bar@tpcBar'),
			]);
			this.attachHover(el.tpcBar, getOverallHoverText);
			return el;
		} else {
			const el = h('.test-coverage-bars', [
				h('.tpc@overall'),
				h('.bar@statement'),
				h('.bar@function'),
				h('.bar@branch'),
			]);
			this.attachHover(el.statement, stmtCoverageText);
			this.attachHover(el.function, fnCoverageText);
			this.attachHover(el.branch, branchCoverageText);
			return el;
		}
	});

	private readonly visibleStore = this._register(new DisposableStore());

	/** Gets whether coverage is currently visible for the resource. */
	public get visible() {
		return !!this._coverage;
	}

	constructor(
		protected readonly options: TestCoverageBarsOptions,
		@IHoverService private readonly hoverService: IHoverService,
		@IConfigurationService private readonly configurationService: IConfigurationService,
	) {
		super();
	}

	private attachHover(target: HTMLElement, factory: (coverage: AbstractFileCoverage) => string | IMarkdownString | undefined) {
		target.onmouseenter = () => {
			if (!this._coverage) {
				return;
			}

			const content = factory(this._coverage);
			if (!content) {
				return;
			}

			const hover = this.hoverService.showHover({
				content,
				target,
				appearance: {
					showPointer: true,
					compact: true,
					skipFadeInAnimation: true,
				}
			});
			if (hover) {
				this.visibleStore.add(hover);
			}
		};
	}

	public setCoverageInfo(coverage: AbstractFileCoverage | undefined) {
		const ds = this.visibleStore;
		if (!coverage) {
			if (this._coverage) {
				this._coverage = undefined;
				ds.clear();
			}
			return;
		}

		if (!this._coverage) {
			const root = this.el.value.root;
			ds.add(toDisposable(() => this.options.container.removeChild(root)));
			this.options.container.appendChild(root);
			ds.add(this.configurationService.onDidChangeConfiguration(c => {
				if (c.affectsConfiguration(TestingConfigKeys.CoveragePercent) && this._coverage) {
					this.doRender(this._coverage);
				}
			}));
		}

		this._coverage = coverage;
		this.doRender(coverage);
	}

	private doRender(coverage: AbstractFileCoverage) {
		const el = this.el.value;

		const precision = this.options.compact ? 0 : 2;
		const overallStat = calculateDisplayedStat(coverage, getTestingConfiguration(this.configurationService, TestingConfigKeys.CoveragePercent));
		el.overall.textContent = displayPercent(overallStat, precision);
		if ('tpcBar' in el) { // compact mode
			renderBar(el.tpcBar, overallStat);
		} else {
			renderBar(el.statement, percent(coverage.statement));
			renderBar(el.function, coverage.function && percent(coverage.function));
			renderBar(el.branch, coverage.branch && percent(coverage.branch));
		}
	}
}

const percent = (cc: ICoveredCount) => cc.total === 0 ? 1 : cc.covered / cc.total;
const epsilon = 10e-8;

const renderBar = (bar: HTMLElement, pct: number | undefined) => {
	if (pct === undefined) {
		bar.style.display = 'none';
	} else {
		bar.style.display = 'block';
		bar.style.setProperty('--test-bar-width', `${pct * 100}%`);
		bar.style.color = `var(${colorThresholds.find(t => pct >= t.threshold)!.color})`;
	}
};

const calculateDisplayedStat = (coverage: AbstractFileCoverage, method: TestingDisplayedCoveragePercent) => {
	switch (method) {
		case TestingDisplayedCoveragePercent.Statement:
			return percent(coverage.statement);
		case TestingDisplayedCoveragePercent.Minimum: {
			let value = percent(coverage.statement);
			if (coverage.branch) { value = Math.min(value, percent(coverage.branch)); }
			if (coverage.function) { value = Math.min(value, percent(coverage.function)); }
			return value;
		}
		case TestingDisplayedCoveragePercent.TotalCoverage:
			return coverage.tpc;
		default:
			assertNever(method);
	}

};

const displayPercent = (value: number, precision = 2) => {
	const display = (value * 100).toFixed(precision);

	// avoid showing 100% coverage if it just rounds up:
	if (value < 1 - epsilon && display === '100') {
		return `${100 - (10 ** -precision)}%`;
	}

	return `${display}%`;
};

const stmtCoverageText = (coverage: AbstractFileCoverage) => localize('statementCoverage', '{0}/{1} statements covered ({2})', coverage.statement.covered, coverage.statement.total, displayPercent(percent(coverage.statement)));
const fnCoverageText = (coverage: AbstractFileCoverage) => coverage.function && localize('functionCoverage', '{0}/{1} functions covered ({2})', coverage.function.covered, coverage.function.total, displayPercent(percent(coverage.function)));
const branchCoverageText = (coverage: AbstractFileCoverage) => coverage.branch && localize('branchCoverage', '{0}/{1} branches covered ({2})', coverage.branch.covered, coverage.branch.total, displayPercent(percent(coverage.branch)));

const getOverallHoverText = (coverage: AbstractFileCoverage) => new MarkdownString([
	stmtCoverageText(coverage),
	fnCoverageText(coverage),
	branchCoverageText(coverage),
].filter(isDefined).join('\n\n'));

/**
 * Renders test coverage bars for a resource in the given container. It will
 * not render anything unless a test coverage report has been opened.
 */
export class ExplorerTestCoverageBars extends ManagedTestCoverageBars implements IExplorerFileContribution {
	private readonly resource = observableValue<URI | undefined>(this, undefined);

	constructor(
		options: TestCoverageBarsOptions,
		@IHoverService hoverService: IHoverService,
		@IConfigurationService configurationService: IConfigurationService,
		@ITestCoverageService testCoverageService: ITestCoverageService,
	) {
		super(options, hoverService, configurationService);

		const isEnabled = observableFromEvent(configurationService.onDidChangeConfiguration, () =>
			getTestingConfiguration(configurationService, TestingConfigKeys.ShowCoverageInExplorer));

		this._register(autorun(async reader => {
			let info: AbstractFileCoverage | undefined;
			const coverage = testCoverageService.selected.read(reader);
			if (coverage && isEnabled.read(reader)) {
				const resource = this.resource.read(reader);
				if (resource) {
					info = coverage.getComputedForUri(resource);
				}
			}

			this.setCoverageInfo(info);
		}));
	}

	/** @inheritdoc */
	public setResource(resource: URI | undefined, transaction?: ITransaction) {
		this.resource.set(resource, transaction);
	}

	public override setCoverageInfo(coverage: AbstractFileCoverage | undefined) {
		super.setCoverageInfo(coverage);
		this.options.container?.classList.toggle('explorer-item-with-test-coverage', this.visible);
	}
}
