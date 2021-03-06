﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { BoardReaderFromUrlState } from './Interfaces';
import { ApiClient } from './ApiClient';

export class BoardReaderFromUrl extends React.Component<RouteComponentProps<{ boardId: string, presentationId: string }>, BoardReaderFromUrlState> {
    refreshTimer: number;

    constructor(props) {
        super(props);
        this.state = {
            boardList: [],
            board: {
                id: "0", name: "", fromCache: false, message: "", columns: [], rows: [], cardColors: [], hasChanged: false
            },
            boardChanged: false,
            loading: true
        };

        ApiClient.getPresentation(this.props.match.params.presentationId)
            .then(dataPres => {

                ApiClient.boardData(this.props.match.params.boardId, this.props.match.params.presentationId)
                    .then(dataBoard => {
                         this.setState({ boardList: dataPres.boards.values, board: dataBoard, boardChanged: true }, this.makeList);
                    });
            });
    }

    makeList() {
        let newBoardList = [];

        this.state.boardList.map(board => {
            if (board.id == this.props.match.params.boardId) {
                newBoardList.push(board);
            }
        });

        this.setState({ boardList: newBoardList, loading: false }, this.RefreshRate);
    }

    boardLoad() {
 
        ApiClient.getPresentation(this.props.match.params.presentationId)
            .then(dataPres => {

                ApiClient.boardData(this.props.match.params.boardId, this.props.match.params.presentationId)
                    .then(dataBoard => {

                         if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == false) {
                             this.setState({ boardChanged: false }, this.RefreshRate);
                         }
                         else {
                             this.setState({ board: dataBoard, boardChanged: true }, this.RefreshRate);
                         }                                                                       
                    });
            });
    }

    saveBoardViewStatistics() {
        let stats = {
            presentationId: this.props.match.params.presentationId,
            boardId: this.state.board.id,
            type: "board"
        }
        ApiClient.saveViewStatistics(stats);
    }

    RefreshRate() {
        this.refreshTimer = setTimeout(this.boardLoad, this.state.boardList[0].refreshRate * 1000);
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
                return <h1>Error loading!(Check CAPTCHA)</h1>
            }
            else {
                return <div>

                    <div>  <BoardName presentationId={this.props.match.params.presentationId} name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} boardlist={this.state.boardList} /></div>
                    <div id='board'><BoardTable board={this.state.board} /></div>

                    {this.saveBoardViewStatistics()}
                </div>;
            }
        }
    }
}






















