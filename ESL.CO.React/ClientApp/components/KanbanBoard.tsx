import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface FetchDataExampleState {
    board: Board;
    loading: boolean;
}

export class KanbanBoard extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        this.state = {
            board: { id: 0, columns: [], rows: [] },
            loading: true
        };

        fetch('api/SampleData/BoardData')
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({ board: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : KanbanBoard.renderBoard(this.state.board);

        return <div>
            <h1>Kanban board</h1>
            {contents}
        </div>;
    }

    private static renderBoard(board: Board) {
        return (
            <div>
                <p>Board: #{board.id}</p>
                <table className='table' >
                    <thead>
                        <tr>
                            {board.columns.map(boardColumn =>
                                <td>{boardColumn.name}</td>
                            )}
                        </tr>
                    </thead>
                    <tbody>
                        {board.rows.map(boardRow =>
                            <tr>
                                {boardRow.issueRow.map(issue =>
                                    <td>
                                        <pre>
                                            <a href={'https://jira.returnonintelligence.com/browse/' + issue.key}>{issue.key}</a>{"\n"}
                                            {issue.fields.summary}{"\n"}
                                            {(issue.fields.assignee) ? issue.fields.assignee.displayName : ""}{"\n"}
                                            {issue.fields.priority.name}{"\n"}
                                        </pre>
                                    </td>
                                )}
                            </tr>
                        )}
                    </tbody>
                </table >
            </div>
        )
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
    displayName: string;
}

interface Priority {
    name: string;
    id: string;
}