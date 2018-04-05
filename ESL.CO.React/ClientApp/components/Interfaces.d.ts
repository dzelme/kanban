import * as React from 'react';

export interface Value {
    id: number;
    name: string;
    type: string;
    visibility: boolean;
    timeShown: number;
    refreshRate: number;
    //timesShown: number;
    //lastShown: string;
}

export interface Board {
    id: number;
    name: string;
    fromCache: boolean;
    message: string;
    columns: BoardColumn[];
    rows: BoardRow[];
    hasChanged: boolean;
}

export interface BoardColumn {
    name: string;
    issues: Issue[];
}

export interface BoardRow {
    issueRow: Issue[];
}

export interface Issue {
    key: string;
    fields: Fields;
}

export interface Fields {
    priority: Priority;
    assignee: Assignee;
    status: Status;
    progress: Progress;
    description: string;
    summary: string;
    created: Date;
    timeTracking: TimeTracking;
    resolutionDate: string;
    dueDate: string;
}

export interface Progress {
    progress: number;
    total: number;
    percent: number;
}

export interface TimeTracking {
    originalEstimateSeconds: number;
    remainingEstimateSeconds: number;
    timeSpentSeconds: number;
}

export interface Status {
    name: string;
    id: string;
}

export interface Assignee {
    displayName: string;
}

export interface Priority {
    name: string;
    id: string;
    color: string;
}

export interface Credentials {
    username: string;
    password: string;
}

export interface FullPresentationList {
    presentationList: BoardPresentation[];
}

export interface FullBoardList {
    values: Value[];
}

export interface BoardPresentation {
    id: string;
    title: string;
    owner: string;
    credentials: Credentials;
    boards: FullBoardList;
}

interface JiraConnectionLogEntry {
    time: string;
    link: string;
    responseStatus: string;
    exception: string;
}

interface StatisticsEntry {
    id: string;
    name: string;
    timesShown: number;
    lastShown: string;
    networkStats: JiraConnectionLogEntry[];
}

interface BoardReaderState {
    boardList: Value[];
    titleList: string[];
    loading: boolean;
}

interface ColumnReaderState {
    boardList: Value[];
    currentIndex: number;
    boardId: number;
    board: Board;
    boardChanged: boolean;
    loading: boolean;
}

interface ReaderFromURLState {
    boardList: Value[];
    board: Board;
    boardChanged: boolean;
    loading: boolean;
}

interface EditPresentationState {
    boardPresentation: BoardPresentation;
    boardList: Value[];
    credentials: Credentials;
    authenticated: boolean;
    loading: boolean;
}

interface CreatePresentationState {
    boardPresentation: BoardPresentation;
    boardList: Value[];
    authenticated: boolean;
    loading: boolean;
}

interface AuthenticationState {
    credentials: Credentials;
    authenticated: boolean;
}

interface PresentationListState {
    presentationList: BoardPresentation[];
    loading: boolean;
}

interface StatisticsListState {
    statsList: StatisticsEntry[];
    loading: boolean;
}

interface StatisticsBoardState {
    connectionLog: JiraConnectionLogEntry[];
    loading: boolean;
}
