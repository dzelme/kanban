﻿import * as React from 'react';

export interface Value {
    id: number;
    name: string;
    type: string;
    visibility: boolean;
    timeShown: number;
    refreshRate: number;
    timesShown: number;
    lastShown: string;
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
}

export interface Progress {
    progress: number;
    total: number;
    percent: number;
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