import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router';
import 'isomorphic-fetch';
import { Link } from 'react-router-dom';

interface FetchDataExampleState {
    boardlist: Value[];
    loading: boolean;
}

export class StatisticsList extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        
        this.state = { boardlist: [], loading: true };

        fetch('api/SampleData/BoardList', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardlist: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : StatisticsList.renderStatisticsList(this.state.boardlist);

        return <div className='top-padding'>
            <h1>Statistics</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsList(boardlist: Value[]) {  //
        return <table className='table'>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Times shown</th>
                        <th>Last shown</th>
                        <th>Network</th>
                    </tr>
                </thead>
                <tbody>
                    {boardlist.map(board =>
                        <tr key={board.id + "row"}>
                            <td key={board.id + ""}>{board.id}</td>
                            <td key={board.id + "name"}>{board.name}</td>
                            <td key={board.id + "timesShown"}>{board.timesShown.toString()}</td>
                            <td key={board.id + "lastShown"}>
                            {board.lastShown ?
                                new Intl.DateTimeFormat('lv-LV', {
                                    day: '2-digit', month: '2-digit', year: 'numeric',
                                    hour: 'numeric', minute: 'numeric', second: 'numeric'
                                }).format(new Date(Date.parse(board.lastShown)))
                                : ""
                            }
                            </td>
                            <td><Link to={'/jiraconnectionstats/' + board.id} className="btn btn-default">Show Jira connections</Link></td>
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

    timesShown: number;
    lastShown: string;
}