import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { ReaderFromURLState } from './Interfaces';
import { ApiClient } from './ApiClient';

export class BoardReaderFromUrl extends React.Component<RouteComponentProps<{ boardId: number, presentationId: string }>, ReaderFromURLState> {
    refreshTimer: number;

    constructor(props) {
        super(props);
        this.state = {
            boardlist:[],
            board: {
                id: 0, name: "", fromCache: false, message: "", columns: [], rows: [], hasChanged: false
            },
            boardChanged: false,
            colorList:[],
            loading: true
        };

        ApiClient.getAPresentation(this.props.match.params.presentationId)
            .then(dataPres => {

                ApiClient.boardData(this.props.match.params.boardId, dataPres.credentials)
                    .then(dataBoard => {

                        ApiClient.colorList(this.props.match.params.boardId, dataPres.credentials)
                            .then(dataColor => {
                                this.setState({ boardlist: dataPres.boards.values, board: dataBoard, boardChanged: true, colorList: dataColor }, this.makeList);
                            });

                    });
            });
    }

    makeList() {
        let newBoardList = [];

        this.state.boardlist.map(board => {
            if (board.id == this.props.match.params.boardId) {
                newBoardList.push(board);
            }
        });

        this.setState({ boardlist: newBoardList, loading: false }, this.RefreshRate);
    }

    boardLoad() {
        clearInterval(this.refreshTimer);

        ApiClient.getAPresentation(this.props.match.params.presentationId)
            .then(dataPres => {

                ApiClient.boardData(this.props.match.params.boardId, dataPres.credentials)
                    .then(dataBoard => {

                        ApiClient.colorList(this.props.match.params.boardId, dataPres.credentials)
                            .then(dataColor => {
                                if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == false && dataColor == this.state.colorList) {
                                    this.setState({ boardChanged: false }, this.RefreshRate);
                                }
                                else {
                                    this.setState({ board: dataBoard, colorList: dataColor, boardChanged: true }, this.RefreshRate);
                                }                             
                            });                                           
                    });
            });
    }

    RefreshRate() {
        this.refreshTimer = setInterval(
            () => this.boardLoad(),
            this.state.boardlist[0].refreshRate
        );
    }

    //AD: increments timesShown board statistic
    increment() {
        ApiClient.incrementTimesShown(this.props.match.params.boardId);
    }

    shouldComponentUpdate(nextProps, nextState) {
        return nextState.boardChanged;
    }

    componentWillUnmount() {
        clearInterval(this.refreshTimer);
    }

    public render() {

        if (this.state.loading) {
            return <h1>Loading...</h1>
        }
        else {

            if (this.state.board.columns.length == 0) {
                return <h1>Error loading!</h1>
            }
            else {
                return <div>

                    <div>  <BoardName presentationId={this.props.match.params.presentationId} name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} boardlist={this.state.boardlist} /></div>
                    <div id='board'><BoardTable board={this.state.board} colorList={this.state.colorList} /></div>

                    {this.increment()}

                </div>;
            }
        }
    }
}






















