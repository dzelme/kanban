﻿import * as React from 'react';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { ColumnReaderState, Value } from './Interfaces';
import { ApiClient } from './ApiClient';

export default class ColumnReader extends React.Component<{ boardList: Value[], presentationId: string, titleList: string[] }, ColumnReaderState> {
    refreshTimer: number;
    showTimer: number;
    
    constructor(props) {
        super(props);
        this.state = {
            presentationId: "",
            boardList: this.props.boardList,
            currentIndex: 0,
            boardId: this.props.boardList[0].id,
            board: {
                id: "0", name: "", fromCache: false, message: "", columns: [], rows: [], cardColors: [], hasChanged: false
            },
            boardChanged: false,
            sameBoard: false,
            loading: true
        };

        this.nextSlide = this.nextSlide.bind(this);
        this.boardLoad = this.boardLoad.bind(this);

        ApiClient.getPresentation(this.props.presentationId)
            .then(dataPres => {

                ApiClient.boardData(this.state.boardId, this.props.presentationId)
                    .then(dataBoard => {
                        this.setState({ board: dataBoard, presentationId: this.props.presentationId, loading: false, boardChanged: true, sameBoard: false }, this.RefreshRate);
                    });
            });     
    }

    nextSlide() {
        clearTimeout(this.refreshTimer);

        var index;

        if (this.state.currentIndex == (this.state.boardList.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }

        const newState = {
            currentIndex: index,
            boardId: this.state.boardList[index].id,
            boardChanged: false
        }

        this.setState(newState, this.boardLoad);
    }

    slideShow() {
        this.saveBoardViewStatistics();
        this.showTimer = setTimeout(this.nextSlide, this.state.boardList[this.state.currentIndex].timeShown * 1000);
    }

    saveBoardViewStatistics() {
        let stats = {
            presentationId: this.state.presentationId,
            boardId: this.state.board.id,
            type: "board"
        }
        ApiClient.saveViewStatistics(stats);
    }

    boardLoad() {

        ApiClient.getPresentation(this.state.presentationId)
            .then(dataPres => {

                ApiClient.boardData(this.state.boardId, this.props.presentationId)
                    .then(dataBoard => {

                        if (dataBoard.id == this.state.boardId) {
                            if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == false) {
                                   this.setState({ boardChanged: false, sameBoard: true }, this.RefreshRate);
                            }
                            else if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == true) {
                                    this.setState({ board: dataBoard, boardChanged: true, sameBoard: true }, this.RefreshRate);
                            }
                             else {
                                 this.setState({ board: dataBoard, boardChanged: true, sameBoard: false }, this.RefreshRate);
                            }
                        }
                    });
            });      
    }

    RefreshRate() {
        this.refreshTimer = setTimeout(this.boardLoad, this.state.boardList[this.state.currentIndex].refreshRate * 1000);
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

                    <div>  <BoardName presentationId={this.props.presentationId} name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} boardlist={this.state.boardList} /></div>
                    <div id='board'><BoardTable board={this.state.board} /></div>

                    {(this.state.boardList.length <= 1) ? this.saveBoardViewStatistics() : this.slideShow()}
                </div>;              
            }
        }
    }
}