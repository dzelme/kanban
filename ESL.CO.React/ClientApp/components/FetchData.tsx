import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import * as ReactDOM from 'react-dom';


export class Clock extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {  //{ date: Date }> {
    timerID: number;
    constructor(props) {
        super(props);
        //this.state = { date: new Date() };
        this.state = {
            board: { id: 0, fromCache: false, message: "", columns: [], rows: [] },
            loading: true
        };

        fetch('api/SampleData/BoardData')
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({ board: data, loading: false });
            });
    }

    componentDidMount() {
        this.timerID = setInterval(
            () => this.tick(),
            10000
        );
    }

    componentWillUnmount() {
        clearInterval(this.timerID);
    }

    tick() {
        /*
        this.setState({
            date: new Date()
        });*/
        fetch('api/SampleData/BoardData')
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({ board: data, loading: false });
            });
    }

    /*
    render() {
        return (
            <div>
                <h1>Hello, world!</h1>
                <h2>It is {this.state.date.toLocaleTimeString()}.</h2>
            </div>
        );
    }
    */
    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Clock.renderBoard(this.state.board);

        return <div>
            <h1>Kanban board</h1>
            {contents}
        </div>;
    }

    private static renderBoard(board: Board) {
        return (
            <div>
                <p>Board: #{board.id}{board.fromCache ? " (from cache)" : " "} {board.message}</p>
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

/*
ReactDOM.render(
    <Clock />,
    document.getElementById('root')
);
*/


interface FetchDataExampleState {
    board: Board;
    loading: boolean;
}
/*
export class KanbanBoard extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        this.state = {
            board: { id: 0, fromCache: false, message: "", columns: [], rows: [] },
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
                <p>Board: #{board.id}{board.fromCache ? " (from cache)" : " "} {board.message}</p>
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
*/
interface Value {
    id: number;
    name: string;
    type: string;
}

interface Board {
    id: number;
    fromCache: boolean;
    message: string;
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















/*
export class FetchData extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        this.state = { forecasts: [], loading: true };

        fetch('api/SampleData/WeatherForecasts')
            .then(response => response.json() as Promise<WeatherForecast[]>)
            .then(data => {
                this.setState({ forecasts: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : FetchData.renderForecastsTable(this.state.forecasts);

        return <div>
            <h1>Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            { contents }
        </div>;
    }

    private static renderForecastsTable(forecasts: WeatherForecast[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
            {forecasts.map(forecast =>
                <tr key={ forecast.dateFormatted }>
                    <td>{ forecast.dateFormatted }</td>
                    <td>{ forecast.temperatureC }</td>
                    <td>{ forecast.temperatureF }</td>
                    <td>{ forecast.summary }</td>
                </tr>
            )}
            </tbody>
        </table>;
    }
}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
*/