import * as React from 'react';

export interface Value {
    id: string;
    name: string;
    visibility: boolean;
    timeShown: number;
    refreshRate: number;
}

export interface Board {
    id: string;
    name: string;
    fromCache: boolean;
    message: string;
    columns: BoardColumn[];
    rows: BoardRow[];
    cardColors: CardColor[];
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

export interface ColorList {
    cardColors: CardColor[];
}

export interface CardColor {
    color: string;
    displayValue: string;
    value: string;
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

interface StatisticsDbModel {
    presentationId: string;
    boardId: string;
    type: string;
}

interface StatisticsPresentationModel {
    presentationId: string;
    title: string;
    boards: FullBoardList;
    timesShown: number;
    lastShown: string;
}

interface StatisticsBoardModel {
    boardId: string;
    boardName: string;
    timesShown: number;
    lastShown: string;
}

interface StatisticsConnectionModel {
    time: string;
    link: string;
    responseStatus: string;
    exception: string;
}

interface BoardReaderState {
    boardList: Value[];
    titleList: string[];
    loading: boolean;
}

interface ColumnReaderState {
    presentationId: string;
    boardList: Value[];
    currentIndex: number;
    boardId: string;
    board: Board;
    boardChanged: boolean;
    sameBoard: boolean;
    loading: boolean;
}

interface BoardReaderFromUrlState {
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

interface StatisticsPresentationListState {
    statisticsPresentationList: StatisticsPresentationModel[];
    loading: boolean;
}

interface StatisticsBoardListState {
    statisticsBoardList: StatisticsBoardModel[];
    loading: boolean;
}

interface StatisticsConnectionListState {
    statisticsConnectionList: StatisticsConnectionModel[];
    loading: boolean;
}
