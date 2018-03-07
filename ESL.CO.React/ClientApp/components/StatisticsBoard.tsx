import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface FetchDataExampleState {
    connectionLog: JiraConnectionLogEntry[];
    loading: boolean;
}

export class StatisticsBoard extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        
        this.state = { connectionLog: [], loading: true };

        fetch('api/SampleData/NetworkStatistics')
            .then(response => response.json() as Promise<JiraConnectionLogEntry[]>)
            .then(data => {
                this.setState({ connectionLog: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : StatisticsBoard.renderStatisticsBoard(this.state.connectionLog);

        return <div>
            <h1>Board #XXX : Network Statistics</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsBoard(connectionLog: JiraConnectionLogEntry[]) {  //
        return <table className='table'>
                <thead>
                    <tr>
                        <th>Time</th>
                        <th>Link</th>
                        <th>Response status</th>
                        <th>Exception</th>
                    </tr>
                </thead>
                <tbody>
                    {connectionLog.map(entry =>
                        <tr key={entry.time + "row"}>
                            <td key={entry.time + "time"}>
                            {entry.time ?
                                new Intl.DateTimeFormat('lv-LV', {
                                    day: '2-digit', month: '2-digit', year: 'numeric',
                                    hour: 'numeric', minute: 'numeric', second: 'numeric'
                                }).format(new Date(Date.parse(entry.time)))
                                : ""
                            }
                            </td>
                            <td key={entry.time + "link"}>{entry.link}</td>
                            <td key={entry.time + "responseStatus"}>{entry.responseStatus}</td>
                            <td key={entry.time + "exception"}>{entry.exception}</td>
                        </tr>
                    )}
                </tbody>
            </table>;
    }
}

interface JiraConnectionLogEntry {
    time: string;
    link: string;
    responseStatus: string;
    exception: string;
}