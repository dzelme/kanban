import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router';
import 'isomorphic-fetch';
import { Link } from 'react-router-dom';
import { Credentials } from './Interfaces';

interface FetchDataExampleState {
    boardlist: Value[];
    loading: boolean;
    credentials: Credentials;
}

interface Value {
    id: number;
    name: string;
    type: string;

    timesShown: number;
    lastShown: string;
}

export class StatisticsList extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        
        this.state = {
            boardlist: [],
            loading: true,
            credentials: { username:"service.kosmoss.tv", password:"ZycsakMylp8od6" }
        };
        this.state = { boardlist: [], loading: true };
    }

    componentWillMount() {
        function handleErrors(response) {
            if (response.status == 401) {
                open('/login', '_self');
                return response;
            }
            if (!response.ok) {
                throw Error(response.statusText);
            }
            return response;
        }

        fetch('api/SampleData/BoardList?credentials=' + this.state.credentials.username + ":" + this.state.credentials.password, {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(handleErrors)
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardlist: data, loading: false });
            });
    }

    public render() {
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }
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
            <thead style={styleHeader}>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Attēlošanas reizes</th>
                        <th>Pēdējo reizi attēlots</th>
                        <th>Savienojums</th>
                    </tr>
            </thead>
            <tbody style={styleContent}>
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
                            <td><Link to={'/admin/jiraconnectionstats/' + board.id} className="btn btn-default">Savienojums</Link></td>
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

const styleHeader = {
    fontSize: '20px'
}

const styleContent = {
    fontSize: '15px'
}