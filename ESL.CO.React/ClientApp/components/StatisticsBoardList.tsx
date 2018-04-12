import * as React from 'react';
import 'isomorphic-fetch';
import { RouteComponentProps} from 'react-router';
import { Link } from 'react-router-dom';
import { StatisticsBoardListState, StatisticsBoardModel } from './Interfaces';
import { ApiClient } from './ApiClient';


export class StatisticsBoardList extends React.Component<RouteComponentProps<{ presentationId: string }>, StatisticsBoardListState> {
    constructor(props: RouteComponentProps<{ presentationId: string }>) {
        super(props);
        
        this.state = {
            statisticsBoardList: [],
            loading: true,
        };
    }

    componentWillMount() {
        ApiClient.statisticsBoardList(this.props.match.params.presentationId)
            .then(data => {
                this.setState({
                    statisticsBoardList: data,
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
            : StatisticsBoardList.renderStatisticsBoardList(this.state.statisticsBoardList, this.props.match.params.presentationId);

        return <div className='top-padding'>
            <h1>Prezentācijas #{this.props.match.params.presentationId} paneļu statistika</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsBoardList(statisticsBoardList: StatisticsBoardModel[], presentationId: string) {  //
        return <table className='table'>
            <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Attēlošanu skaits</th>
                        <th>Pēdējā attēlošana</th>
                        <th>Savienojums</th>
                    </tr>
            </thead>
            <tbody>
                    {statisticsBoardList.map(board =>
                        <tr key={board.boardId + "row"}>
                            <td key={board.boardId + ""}>{board.boardId}</td>
                            <td key={board.boardId + "name"}>{board.boardId}</td>
                            <td key={board.boardId + "timesShown"}>{board.timesShown.toString()}</td>
                            <td key={board.boardId + "lastShown"}>{board.lastShown}</td>
                            <td><Link to={'/admin/statistics/' + presentationId + '/' + board.boardId}><button className="btn btn-default">Savienojums</button></Link></td>
                        </tr>
                    )}
                </tbody>
            </table>;
    }
}