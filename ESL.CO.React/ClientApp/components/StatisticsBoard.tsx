import * as React from 'react';
import 'isomorphic-fetch';
import { withRouter } from 'react-router';
import { RouteComponentProps } from 'react-router';
import { StatisticsBoardState, JiraConnectionLogEntry } from './Interfaces';
import { ApiClient } from './ApiClient';

//netiek sanemti props (undefined) no routes.tsx
export class StatisticsBoard extends React.Component<RouteComponentProps<{ id: string }>, FetchDataExampleState> {
    constructor(props: RouteComponentProps<{ id: string }>) {
        super(props);
        this.state = { connectionLog: [], loading: true };
    }

    componentWillMount() {
        ApiClient.networkStatistics(this.props.match.params.id)
            .then(data => {
                this.setState({ connectionLog: data, loading: false });
            });
    }

    public render() {
        if (sessionStorage.getItem(ApiClient.tokenName) === null) {
            return null;
        }
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : StatisticsBoard.renderStatisticsBoard(this.state.connectionLog);

        return <div className='top-padding'>
            <h1>Paneļa #{this.props.match.params.id} : Savienojumi</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsBoard(connectionLog: JiraConnectionLogEntry[]) {  //


        return <table className='table'>
            <thead>
                    <tr>
                        <th>Laiks</th>
                        <th>Saite</th>
                        <th>Atbildes statuss</th>
                        <th>Izņēmums</th>
                    </tr>
            </thead>
            <tbody>
                    {connectionLog.map(entry =>
                        <tr key={entry.time + entry.link + "row"}>
                            <td key={entry.time + "time"}>{entry.time}</td>
                            <td key={entry.time + "link"}>{entry.link}</td>
                            <td key={entry.time + "responseStatus"}>{entry.responseStatus}</td>
                            <td key={entry.time + "exception"}>{entry.exception}</td>
                        </tr>
                    )}
                </tbody>
            </table>;
    }
}