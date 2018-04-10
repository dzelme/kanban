import * as React from 'react';
import 'isomorphic-fetch';
import { RouteComponentProps} from 'react-router';
import { Link } from 'react-router-dom';
import { StatisticsListState, Credentials, StatisticsModel } from './Interfaces';
import { ApiClient } from './ApiClient';


export class StatisticsList extends React.Component<RouteComponentProps<{}>, StatisticsListState> {
    constructor() {
        super();
        
        this.state = {
            statsList: [],
            loading: true,
        };
    }

    componentWillMount() {
        ApiClient.statisticsList()
            .then(data => {
                this.setState({
                    statsList: data,
                    loading: false,
                })
            });
    }

    public render() {
        if (sessionStorage.getItem(ApiClient.tokenName) === null) {
            return null;
        }
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : StatisticsList.renderStatisticsList(this.state.statsList);

        return <div className='top-padding'>
            <h1>Statistika</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsList(statsList: StatisticsModel[]) {  //
        return <table className='table'>
            <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Attēlošanas reizes</th>
                        <th>Pēdējo reizi attēlots</th>
                        <th>Savienojums</th>
                    </tr>
            </thead>
            <tbody>
                    {statsList.map(entry =>
                        <tr key={entry.boardId + "row"}>
                            <td key={entry.boardId + ""}>{entry.boardId}</td>
                            <td key={entry.boardId + "name"}>{entry.boardId}</td>
                            <td key={entry.boardId + "timesShown"}>{entry.timesShown.toString()}</td>
                            <td key={entry.boardId + "lastShown"}>{entry.lastShown}</td>
                            <td><Link to={'/admin/statistics/' + entry.boardId}><button className="btn btn-default">Savienojums</button></Link></td>
                        </tr>
                    )}
                </tbody>
            </table>;
    }
}