﻿import * as React from 'react';
import 'isomorphic-fetch';
import { RouteComponentProps} from 'react-router';
import { Link } from 'react-router-dom';
import { StatisticsPresentationListState, StatisticsPresentationModel } from './Interfaces';
import { ApiClient } from './ApiClient';


export class StatisticsPresentationList extends React.Component<RouteComponentProps<{}>, StatisticsPresentationListState> {
    constructor() {
        super();
        
        this.state = {
            statisticsPresentationList: [],
            loading: true,
        };
    }

    componentWillMount() {
        ApiClient.statisticsPresentationList()
            .then(data => {
                this.setState({
                    statisticsPresentationList: data,
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
            : StatisticsPresentationList.renderStatisticsPresentationList(this.state.statisticsPresentationList);

        return <div className='top-padding'>
            <h1>Statistika</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsPresentationList(statisticsPresentationList: StatisticsPresentationModel[]) {  //
        return <table className='table'>
            <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Paneļi</th>
                        <th>Attēlošanu skaits</th>
                        <th>Pēdējā attēlošana</th>
                        <th>Paneļu statistika</th>
                    </tr>
            </thead>
            <tbody>
                    {statisticsPresentationList.map(presentation =>
                        <tr key={presentation.itemId + "row"}>
                            <td key={presentation.itemId + ""}>{presentation.itemId}</td>
                            <td key={presentation.itemId + "title"}>{presentation.title}</td>
                            <td key={presentation.itemId + "boards"}>
                                {presentation.boards.values.map((board, index) =>
                                    <Link key={index} className="LinkText" to={'/admin/statistics/' + presentation.itemId + "/" + board.id}>{board.name}; </Link>
                                )}
                            </td>
                            <td key={presentation.itemId + "timesShown"}>{presentation.timesShown.toString()}</td>
                            <td key={presentation.itemId + "lastShown"}>{presentation.lastShown}</td>
                            <td><Link to={'/admin/statistics/' + presentation.itemId}><button className="btn btn-default">Paneļu statistika</button></Link></td>
                        </tr>
                    )}
                </tbody>
            </table>;
    }
}