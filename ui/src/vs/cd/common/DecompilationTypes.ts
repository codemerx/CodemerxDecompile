export interface AssemblyRelatedFilePaths {
	decompiledAssemblyDirectory: string;
	decompiledAssemblyPath: string;
}

export interface TypeFilePath {
	typeFullName: string;
	absoluteFilePath: string;
}

export interface Selection {
	startLineNumber: number;
	endLineNumber: number;
	startColumn: number;
	endColumn: number;
}

export interface ReferenceMetadata {
	memberFullName: string;
	definitionFilePath?: string;
	isCrossAssemblyReference: boolean;
    referencedAssemblyFullName?: string;
    referencedAssemblyFilePath?: string;
}
