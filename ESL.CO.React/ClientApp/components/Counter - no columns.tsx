import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface FetchDataExampleState {
    //forecasts: WeatherForecast[];
    //loading: boolean;

    boardrows: BoardRow[];
    loading: boolean;
}

export class Counter extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        this.state = { boardrows: [], loading: true };

        fetch('api/SampleData/BoardData')
            .then(response => response.json() as Promise<BoardRow[]>)
            .then(data => {
                this.setState({ boardrows: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Counter.renderBoard(this.state.boardrows);

        return <div>
            <h1>Kanban board</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>;
    }

    private static renderBoard(boardrows: BoardRow[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <td>Draft</td>
                    <td>To do</td>
                    <td>In progress</td>
                    <td>Done</td>
                </tr>
            </thead>
            <tbody>
                {boardrows.map(boardrow =>
                    <tr>
                        {boardrow.issueRow.map(issue =>
                            <td>{issue.key}</td>
                        )}
                    </tr>
                )}
            </tbody>
        </table>;
    }


}

interface Value {
    id: number;
    name: string;
    type: string;
}

interface Board {
    id: number;
    columns: BoardColumn[];
    rows: BoardRow[];
}

interface BoardColumn {
    name: string;
    issues: Issue[];
}

interface BoardRow {
    issueRow: Issue[];
}

interface Issue {
    key: string;
    fields: Fields;
}

interface Fields {
    priority: Priority;
    assignee: Assignee;
    status: Status;
    description: string;
    summary: string;
}

interface Status {
    name: string;
    id: string;
}

interface Assignee {
    displayname: string;
}

interface Priority {
    name: string;
    id: string;
}