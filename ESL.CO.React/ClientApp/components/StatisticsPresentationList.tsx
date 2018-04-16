import * as React from 'react';
import 'isomorphic-fetch';
import { RouteComponentProps} from 'react-router';
import { Link } from 'react-router-dom';
import { StatisticsPresentationListState, StatisticsPresentationModel } from './Interfaces';
import { ApiClient } from './ApiClient';
import { HelperFunctions } from './HelperFunctions';


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
            <h1>Prezentāciju statistika</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsPresentationList(statisticsPresentationList: StatisticsPresentationModel[]) {
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
                        <tr key={presentation.presentationId + "row"}>
                            <td key={presentation.presentationId + ""}>{presentation.presentationId}</td>
                            <td key={presentation.presentationId + "title"}>{presentation.title}</td>
                            <td key={presentation.presentationId + "boards"}>
                                {presentation.boards.values.map((board, index) =>
                                    <Link key={index} className="LinkText" to={'/admin/statistics/' + presentation.presentationId + "/" + board.id}>{board.id}; </Link>
                                )}
                            </td>
                            <td key={presentation.presentationId + "timesShown"}>{presentation.timesShown.toString()}</td>
                            <td key={presentation.presentationId + "lastShown"}>{HelperFunctions.formatDate(presentation.lastShown)}</td>
                            <td><Link to={'/admin/statistics/' + presentation.presentationId}><button className="btn btn-default">Apskatīt</button></Link></td>
                        </tr>
                    )}
                </tbody>
            </table>;
    }
}