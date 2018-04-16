import * as React from 'react';
import 'isomorphic-fetch';
import { withRouter } from 'react-router';
import { RouteComponentProps } from 'react-router';
import { StatisticsConnectionListState, StatisticsConnectionModel } from './Interfaces';
import { ApiClient } from './ApiClient';
import { HelperFunctions } from './HelperFunctions';

//netiek sanemti props (undefined) no routes.tsx
export class StatisticsConnectionList extends React.Component<RouteComponentProps<{ presentationId: string, boardId: string }>, StatisticsConnectionListState> {
    constructor(props: RouteComponentProps<{ presentationId: string, boardId: string }>) {
        super(props);
        this.state = {
            statisticsConnectionList: [],
            loading: true
        };
    }

    componentWillMount() {
        ApiClient.statisticsConnectionList(this.props.match.params.presentationId, this.props.match.params.boardId)
            .then(data => {
                this.setState({ statisticsConnectionList: data, loading: false });
            });
    }

    public render() {
        if (sessionStorage.getItem(ApiClient.tokenName) === null) {
            return null;
        }
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : StatisticsConnectionList.renderStatisticsBoard(this.state.statisticsConnectionList);

        return <div className='top-padding'>
            <h1>Prezentācijas #{this.props.match.params.presentationId} paneļa #{this.props.match.params.boardId} pieprasījumu statistika</h1>
            {contents}
        </div>;
    }

    private static renderStatisticsBoard(statisticsConnectionList: StatisticsConnectionModel[]) {
        
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
                    {statisticsConnectionList.map(entry =>
                        <tr key={entry.time + entry.link + "row"}>
                            <td key={entry.time + "time"}>{HelperFunctions.formatDate(entry.time)}</td>
                            <td key={entry.time + "link"}>{entry.link}</td>
                            <td key={entry.time + "responseStatus"}>{entry.responseStatus}</td>
                            <td key={entry.time + "exception"}>{entry.exception}</td>
                        </tr>
                    )}
                </tbody>
            </table>;
    }
}